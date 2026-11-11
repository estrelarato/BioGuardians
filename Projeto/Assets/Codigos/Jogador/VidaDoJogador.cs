using UnityEngine;
using TMPro;

public class VidaDoJogador : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 20;
    [SerializeField] private int vidaAtual; // Visível no Inspector para acompanhares em Play Mode

    [Header("Interface de Usuário (Apenas Texto)")]
    public TextMeshProUGUI textoVida;

    void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarUI();
        Debug.Log("Jogo Iniciado! Vida da Base: " + vidaAtual);
    }

    void Update()
    {
        // ATALHO DE TESTE: Pressiona a tecla K para simular 5 de dano na base
        if (Input.GetKeyDown(KeyCode.K))
        {
            TomarDano(10);
        }
    }

    public void TomarDano(int quantidade)
    {
        if (vidaAtual <= 0) return;

        vidaAtual -= quantidade;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);

        AtualizarUI();
        Debug.Log("A base sofreu dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            // Avisa o GameManager para disparar a derrota
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PerderJogo();
            }
        }
    }

    void AtualizarUI()
    {
        if (textoVida != null)
        {
            textoVida.text = vidaAtual.ToString();
        }
    }

    // Métodos para consultar a vida se precisares noutros scripts
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMaxima() => vidaMaxima;
}
/* Este é o script oficial, o de cima foi utilizado para testes, mas decidi mantelo pelo mesmo motivo ok
 
using UnityEngine;
using TMPro;

public class VidaDoJogador : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 20;
    private int vidaAtual;

    [Header("Interface de Usuário (Apenas Texto)")]
    public TextMeshProUGUI textoVida;

    void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarUI();
        Debug.Log("Jogo Iniciado! Vida da Base: " + vidaAtual);
    }

    public void TomarDano(int quantidade)
    {
        if (vidaAtual <= 0) return;

        vidaAtual -= quantidade;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);

        AtualizarUI();
        Debug.Log("A base sofreu dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            // Avisa o GameManager para disparar a derrota
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PerderJogo();
            }
        }
    }

    void AtualizarUI()
    {
        if (textoVida != null)
        {
            textoVida.text = vidaAtual.ToString();
        }
    }

    // Métodos para consultar a vida se precisares noutros scripts
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMaxima() => vidaMaxima;
}*/