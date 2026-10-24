using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Interfaces de Fim de Jogo")]
    public GameObject painelVitoria;
    public GameObject painelGameOver;

    [Header("Sistema de Economia")]
    public int dinheiroInicial = 150;
    private int dinheiroAtual;

    [Header("Interface de Economia")]
    public TextMeshProUGUI textoDinheiro;

    private bool checarVitoria = false;
    private bool jogoFinalizado = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        dinheiroAtual = dinheiroInicial;
        
        AtualizarTextoInterface();

        if (painelVitoria != null) painelVitoria.SetActive(false);
        if (painelGameOver != null) painelGameOver.SetActive(false);
    }

    void Update()
    {
        if (checarVitoria && !jogoFinalizado)
        {
            ChecarCondicaoVitoria();
        }
    }

    public void GanharDinheiro(int quantidade)
    {
        if (jogoFinalizado) return;
        dinheiroAtual += quantidade;
        AtualizarTextoInterface();
        Debug.Log("Ganhaste " + quantidade + " moedas! Saldo atual: " + dinheiroAtual);
    }

    public bool TentarGastarDinheiro(int custo)
    {
        if (jogoFinalizado) return false;

        if (dinheiroAtual >= custo)
        {
            dinheiroAtual -= custo;
            AtualizarTextoInterface();
            Debug.Log("Gastaste " + custo + " moedas. Saldo restante: " + dinheiroAtual);
            return true;
        }
        else
        {
            Debug.LogWarning("Dinheiro insuficiente! Custo: " + custo + " | Saldo: " + dinheiroAtual);
            return false;
        }
    }

    void AtualizarTextoInterface()
    {
        if (textoDinheiro != null)
        {
            textoDinheiro.text = dinheiroAtual.ToString();
        }
    }

    public void AtivarChecagemVitoria()
    {
        checarVitoria = true;
    }

    void ChecarCondicaoVitoria()
    {
        Inimigo inimigoRestante = FindFirstObjectByType<Inimigo>();

        if (inimigoRestante == null)
        {
            checarVitoria = false; 
            GanharJogo();
        }
    }

    void GanharJogo()
    {
        jogoFinalizado = true;
        Debug.Log("Parabéns! Defendeu o organismo de todas as ameaças!");
        
        if (painelVitoria != null)
        {
            painelVitoria.SetActive(true);
        }

        Time.timeScale = 0f; 
    }

    public void PerderJogo()
    {
        if (jogoFinalizado) return;
        jogoFinalizado = true;
        
        Debug.Log("Game Over! O organismo foi infectado.");

        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}