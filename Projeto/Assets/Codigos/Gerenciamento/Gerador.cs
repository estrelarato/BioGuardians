using UnityEngine;
using UnityEngine.InputSystem;

public class Gerador : MonoBehaviour
{
    [Header("Prefabs das Torres")]
    public GameObject prefabPadrao;
    public GameObject prefabTorreATP;
    public GameObject prefabTorreLento;

    private GameObject objetoAtual;
    private int custoObjetoAtual; // Armazena o custo da torre que está sendo arrastada

    void Update()
    {
        if (objetoAtual == null)
            return;

        // Converte a posição do cursor/mouse para o mundo 2D
        Vector3 mundo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mundo.z = 0;

        objetoAtual.transform.position = mundo;

        // --- CANCELAR COMPRA (Botão Direito do Mouse ou Tecla ESC) ---
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

                // Finaliza a construção
                DefinirEstadoConstrucao(objetoAtual, false);

                // Reativa TODOS os colisores da torre
                Collider2D[] colisores = objetoAtual.GetComponents<Collider2D>();
                foreach (Collider2D col in colisores)
                {
                    col.enabled = true;
                }

                objetoAtual = null;
                custoObjetoAtual = 0;
            }
        }
    }

    /// <summary>
    /// Cancela a construção atual, devolve o ATP e destrói o objeto fantasma
    /// </summary>
    public void CancelarConstrucao()
    {
        if (objetoAtual != null)
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null && custoObjetoAtual > 0)
            {
                // Devolve o dinheiro gasto ao jogador
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

        // Se já tiver uma torre no cursor, cancela a anterior antes de criar a nova
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
                custoObjetoAtual = custoTorre; // Guarda quanto custou para eventual reembolso

                DefinirEstadoConstrucao(objetoAtual, true);

                // Desativa todos os colisores durante o arraste
                Collider2D[] colisores = objetoAtual.GetComponents<Collider2D>();
                foreach (Collider2D col in colisores)
                {
                    col.enabled = false;
                }
            }
        }
    }

    private void DefinirEstadoConstrucao(GameObject torreObj, bool estado)
    {
        Torre torreNormal = torreObj.GetComponent<Torre>();
        if (torreNormal != null) torreNormal.emConstrucao = estado;

        TorreN torreNova = torreObj.GetComponent<TorreN>();
        if (torreNova != null) torreNova.emConstrucao = estado;

        TorreMacrofaga torreMacrofaga = torreObj.GetComponent<TorreMacrofaga>();
        if (torreMacrofaga != null) torreMacrofaga.emConstrucao = estado;

        TorreGeradoraATP torreATP = torreObj.GetComponent<TorreGeradoraATP>();
        if (torreATP != null) torreATP.emConstrucao = estado;

        TorreDesaceleradora torreLento = torreObj.GetComponent<TorreDesaceleradora>();
        if (torreLento != null) torreLento.emConstrucao = estado;
    }

    private int ObterCustoDaTorre(GameObject prefab)
    {
        Torre torreNormal = prefab.GetComponent<Torre>();
        if (torreNormal != null) return torreNormal.custo;

        TorreN torreNova = prefab.GetComponent<TorreN>();
        if (torreNova != null) return torreNova.custo;

        TorreMacrofaga torreMacrofaga = prefab.GetComponent<TorreMacrofaga>();
        if (torreMacrofaga != null) return torreMacrofaga.custo;

        TorreGeradoraATP torreATP = prefab.GetComponent<TorreGeradoraATP>();
        if (torreATP != null) return torreATP.custo;

        TorreDesaceleradora torreLento = prefab.GetComponent<TorreDesaceleradora>();
        if (torreLento != null) return torreLento.custo;

        return 0;
    }
}