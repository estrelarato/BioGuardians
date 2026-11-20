using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Interface do Pause")]
    public GameObject painelPause;

    [Header("Navegação")]
    [Tooltip("Índice da cena Menu no Build Settings (padrão = 1)")]
    public int buildIndexMenu = 1;

    private bool jogoPausado = false;

    void Start()
    {
        if (painelPause != null)
            painelPause.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
        // Evita pausar por cima da tela de Vitória ou Game Over
        if (Time.timeScale == 0f && !jogoPausado) return;

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

    public void VoltarParaMenu()
    {
        Time.timeScale = 1f; // Sempre descongela o tempo antes de trocar de cena!
        SceneManager.LoadScene(buildIndexMenu);
    }

    public void SairDoJogo()
    {
        Debug.Log("[SISTEMA] Saindo do jogo...");
        Application.Quit();
    }
}