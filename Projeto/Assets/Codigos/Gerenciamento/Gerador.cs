using UnityEngine;
using UnityEngine.InputSystem;

public class Gerador : MonoBehaviour
{
    [Header("Prefabs das Torres")]
    public GameObject prefabPadrao;
    public GameObject prefabTorreATP;
    public GameObject prefabTorreLento;

    private GameObject objetoAtual;
    private int custoObjetoAtual;

    void Update()
    {
        if (objetoAtual == null)
            return;

        Vector3 mundo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mundo.z = 0;

        objetoAtual.transform.position = mundo;

        // --- CANCELAR COMPRA (Botão Direito do Mouse ou ESC) ---
        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelarConstrucao();
            return;
        }

        // --- CONFIRMAR POSICIONAMENTO (Botão Esquerdo do Mouse) ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D local = Physics2D.OverlapPoint(mundo);

            if (local != null && local.CompareTag("LocalDeConstrucao"))
            {
                objetoAtual.transform.position = local.transform.position;

                // Finaliza a construção usando a classe base
                DefinirEstadoConstrucao(objetoAtual, false);

                // Reativa todos os colisores no objeto e nos seus filhos
                Collider2D[] colisores = objetoAtual.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in colisores)
                {
                    col.enabled = true;
                }

                objetoAtual = null;
                custoObjetoAtual = 0;
            }
        }
    }

    public void CancelarConstrucao()
    {
        if (objetoAtual != null)
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null && custoObjetoAtual > 0)
            {
                gameManager.GanharDinheiro(custoObjetoAtual);
            }

            Destroy(objetoAtual);
            objetoAtual = null;
            custoObjetoAtual = 0;

            Debug.Log("[GERADOR] Compra cancelada e valor reembolsado!");
        }
    }

    #region Métodos de UI

    public void CriarObjeto() => CriarTorreEspecifica(prefabPadrao);
    public void CriarTorreATP() => CriarTorreEspecifica(prefabTorreATP);
    public void CriarTorreLento() => CriarTorreEspecifica(prefabTorreLento);

    #endregion

    public void CriarTorreEspecifica(GameObject prefabEscolhido)
    {
        if (prefabEscolhido == null) return;

        if (objetoAtual != null)
        {
            CancelarConstrucao();
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        int custoTorre = ObterCustoDaTorre(prefabEscolhido);

        if (gameManager != null && custoTorre > 0)
        {
            if (gameManager.TentarGastarDinheiro(custoTorre))
            {
                objetoAtual = Instantiate(prefabEscolhido);
                custoObjetoAtual = custoTorre;

                DefinirEstadoConstrucao(objetoAtual, true);

                // Desativa todos os colisores durante o arraste
                Collider2D[] colisores = objetoAtual.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in colisores)
                {
                    col.enabled = false;
                }
            }
        }
    }

    private void DefinirEstadoConstrucao(GameObject torreObj, bool estado)
    {
        TorreBase torre = torreObj.GetComponent<TorreBase>();
        if (torre != null)
        {
            torre.emConstrucao = estado;
        }
    }

    private int ObterCustoDaTorre(GameObject prefab)
    {
        TorreBase torre = prefab.GetComponent<TorreBase>();
        return torre != null ? torre.custo : 0;
    }
}