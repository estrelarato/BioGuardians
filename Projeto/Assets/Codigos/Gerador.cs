using UnityEngine;
using UnityEngine.InputSystem;

public class Gerador : MonoBehaviour
{
    [Header("Prefabs das Torres")]
    public GameObject prefabPadrao;
    public GameObject prefabTorreATP;
    public GameObject prefabTorreLento;

    private GameObject objetoAtual;

    void Update()
    {
        if (objetoAtual == null)
            return;

        // Converte a posição do cursor/mouse para o mundo 2D
        Vector3 mundo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mundo.z = 0;

        objetoAtual.transform.position = mundo;

        // Ao clicar com o botão esquerdo para posicionar a torre
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D local = Physics2D.OverlapPoint(mundo);

            if (local != null && local.CompareTag("LocalDeConstrucao"))
            {
                objetoAtual.transform.position = local.transform.position;

                // --- FINALIZA A CONSTRUÇÃO ---
                DefinirEstadoConstrucao(objetoAtual, false);

                // Reativa TODOS os colisores da torre assim que é posicionada
                Collider2D[] colisores = objetoAtual.GetComponents<Collider2D>();
                foreach (Collider2D col in colisores)
                {
                    col.enabled = true;
                }

                objetoAtual = null;
            }
        }
    }

    #region Métodos de Chamada para Botões da UI

    public void CriarObjeto()
    {
        CriarTorreEspecifica(prefabPadrao);
    }

    public void CriarTorreATP()
    {
        CriarTorreEspecifica(prefabTorreATP);
    }

    public void CriarTorreLento()
    {
        CriarTorreEspecifica(prefabTorreLento);
    }

    #endregion

    public void CriarTorreEspecifica(GameObject prefabEscolhido)
    {
        if (prefabEscolhido == null) return;

        if (objetoAtual == null)
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            int custoTorre = ObterCustoDaTorre(prefabEscolhido);

            if (gameManager != null && custoTorre > 0)
            {
                if (gameManager.TentarGastarDinheiro(custoTorre))
                {
                    objetoAtual = Instantiate(prefabEscolhido);

                    // --- INICIA A CONSTRUÇÃO ---
                    DefinirEstadoConstrucao(objetoAtual, true);

                    // Desativa TODOS os colisores para impedir que a torre interaja/abra menu enquanto é arrastada
                    Collider2D[] colisores = objetoAtual.GetComponents<Collider2D>();
                    foreach (Collider2D col in colisores)
                    {
                        col.enabled = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Altera o estado 'emConstrucao' do script presente no prefab da torre
    /// </summary>
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

    /// <summary>
    /// Procura o custo configurado na torre correspondente
    /// </summary>
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