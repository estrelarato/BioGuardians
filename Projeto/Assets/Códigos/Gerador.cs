using UnityEngine;
using UnityEngine.InputSystem;

public class Gerador : MonoBehaviour
{
    public GameObject prefabPadrao; 
    private GameObject objetoAtual;

    void Update()
    {
        if (objetoAtual == null)
            return;

        Vector3 mundo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mundo.z = 0;

        objetoAtual.transform.position = mundo;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D local = Physics2D.OverlapPoint(mundo);

            if (local != null && local.CompareTag("LocalDeConstrucao"))
            {
                objetoAtual.transform.position = local.transform.position;

                // --- FINALIZA A CONSTRUÇÃO (Suporta Torre e TorreN) ---
                Torre torreNormal = objetoAtual.GetComponent<Torre>();
                if (torreNormal != null)
                {
                    torreNormal.emConstrucao = false;
                }

                TorreN torreNova = objetoAtual.GetComponent<TorreN>();
                if (torreNova != null)
                {
                    torreNova.emConstrucao = false;
                }

                // Ativa o colisor da torre posicionada
                Collider2D colisorTorre = objetoAtual.GetComponent<Collider2D>();
                if (colisorTorre != null)
                {
                    colisorTorre.enabled = true;
                }

                objetoAtual = null;
            }
        }
    }

    public void CriarObjeto()
    {
        CriarTorreEspecifica(prefabPadrao);
    }

    public void CriarTorreEspecifica(GameObject prefabEscolhido)
    {
        if (prefabEscolhido == null) return;

        if (objetoAtual == null)
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            
            // Pega o custo de qualquer um dos dois tipos de torre
            int custoTorre = ObterCustoDaTorre(prefabEscolhido);

            if (gameManager != null && custoTorre > 0)
            {
                if (gameManager.TentarGastarDinheiro(custoTorre))
                {
                    objetoAtual = Instantiate(prefabEscolhido);

                    // --- INICIA A CONSTRUÇÃO (Suporta Torre e TorreN) ---
                    Torre torreNormal = objetoAtual.GetComponent<Torre>();
                    if (torreNormal != null)
                    {
                        torreNormal.emConstrucao = true;
                    }

                    TorreN torreNova = objetoAtual.GetComponent<TorreN>();
                    if (torreNova != null)
                    {
                        torreNova.emConstrucao = true;
                    }

                    // Desativa o colisor durante o posicionamento
                    Collider2D colisorTorre = objetoAtual.GetComponent<Collider2D>();
                    if (colisorTorre != null)
                    {
                        colisorTorre.enabled = false;
                    }
                }
            }
        }
    }

    // Função auxiliar para verificar o custo independente do script da torre
    private int ObterCustoDaTorre(GameObject prefab)
    {
        Torre torreNormal = prefab.GetComponent<Torre>();
        if (torreNormal != null) return torreNormal.custo;

        TorreN torreNova = prefab.GetComponent<TorreN>();
        if (torreNova != null) return torreNova.custo;

        return 0; // Caso o prefab não tenha script de torre reconhecido
    }
}