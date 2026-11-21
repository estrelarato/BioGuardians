using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instancia { get; private set; }

    [Header("Interface do Pause")]
    public GameObject painelPause;
    [Tooltip("Arraste aqui o painel dos botões das torres para esconder ao abrir Opções")]
    public GameObject painelTorresHUD;

    [Header("Nomes / Índices das Cenas")]
    public string nomeCenaOpcoes = "Opções";
    public int buildIndexMenu = 1;

    private bool jogoPausado = false;
    private bool opcoesAbertas = false;

    // Propriedade pública para bloquear cliques no mundo/torres enquanto pausado
    public bool EstaPausado => jogoPausado || opcoesAbertas;

    void Awake()
    {
        Instancia = this;
    }

    void OnDestroy()
    {
        if (Instancia == this)
        {
            Instancia = null;
        }
    }

    void Start()
    {
        if (painelPause != null)
            painelPause.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (opcoesAbertas)
                FecharOpcoes();
            else
                AlternarPause();
        }
    }

    public void AlternarPause()
    {
        if (jogoPausado)
            ContinuarJogo();
        else
            PausarJogo();
    }

    public void PausarJogo()
    {
        if (Time.timeScale == 0f && !jogoPausado) return;

        // Fecha o painel de venda de torre caso estivesse aberto antes de pausar
        MenuTorreUI menuTorre = FindFirstObjectByType<MenuTorreUI>();
        if (menuTorre != null)
        {
            menuTorre.FecharMenu();
        }

        jogoPausado = true;
        if (painelPause != null) painelPause.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinuarJogo()
    {
        jogoPausado = false;
        if (painelPause != null) painelPause.SetActive(false);
        Time.timeScale = 1f;
    }

    public void AbrirOpcoes()
    {
        if (opcoesAbertas) return;

        opcoesAbertas = true;
        if (painelPause != null) painelPause.SetActive(false);
        if (painelTorresHUD != null) painelTorresHUD.SetActive(false);

        SceneManager.LoadScene(nomeCenaOpcoes, LoadSceneMode.Additive);
    }

    public void FecharOpcoes()
    {
        if (!opcoesAbertas) return;

        opcoesAbertas = false;
        SceneManager.UnloadSceneAsync(nomeCenaOpcoes);

        if (painelPause != null) painelPause.SetActive(true);
        if (painelTorresHUD != null) painelTorresHUD.SetActive(true);
    }

    public void VoltarParaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndexMenu);
    }

    public void SairDoJogo()
    {
        Debug.Log("[SISTEMA] Saindo do jogo...");
        Application.Quit();
    }
}