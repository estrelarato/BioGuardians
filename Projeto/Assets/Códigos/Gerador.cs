using UnityEngine;
using UnityEngine.InputSystem;

public class Gerador : MonoBehaviour
{
    // Este pode continuar aqui como um padrão, mas não ficaremos presos a ele
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

                Torre torre = objetoAtual.GetComponent<Torre>();

                if (torre != null)
                {
                    torre.emConstrucao = false;
                    
                    Collider2D colisorTorre = objetoAtual.GetComponent<Collider2D>();
                    if (colisorTorre != null)
                    {
                        colisorTorre.enabled = true;
                    }
                }

                objetoAtual = null;
            }
        }
    }

    // Mantive a sua função original caso você já a use em algum lugar
    public void CriarObjeto()
    {
        CriarTorreEspecifica(prefabPadrao);
    }

    // --- NOVA FUNÇÃO INTELIGENTE PARA OS BOTÕES ---
    // Esta função recebe o Prefab direto do botão que foi clicado!
    public void CriarTorreEspecifica(GameObject prefabEscolhido)
    {
        if (prefabEscolhido == null) return;

        if (objetoAtual == null)
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            Torre componenteTorre = prefabEscolhido.GetComponent<Torre>();

            if (gameManager != null && componenteTorre != null)
            {
                // Tenta gastar o dinheiro baseado no custo da torre escolhida
                if (gameManager.TentarGastarDinheiro(componenteTorre.custo))
                {
                    objetoAtual = Instantiate(prefabEscolhido);

                    Torre torre = objetoAtual.GetComponent<Torre>();

                    if (torre != null)
                    {
                        torre.emConstrucao = true;

                        Collider2D colisorTorre = objetoAtual.GetComponent<Collider2D>();
                        if (colisorTorre != null)
                        {
                            colisorTorre.enabled = false;
                        }
                    }
                }
            }
        }
    }
}