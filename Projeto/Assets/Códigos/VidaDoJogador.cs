using UnityEngine;
using TMPro; // Gerencia os textos do TextMeshPro

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
        // Atualiza apenas o componente de Texto do TextMeshPro
        if (textoVida != null)
        {
            // Exibe o número da vida atual na tela (ex: 20, 15, 5, 0)
            textoVida.text = vidaAtual.ToString();
            
            // Caso prefira ver no formato "20 / 20", você pode trocar a linha acima por:
            // textoVida.text = vidaAtual + " / " + vidaMaxima;
        }
    }

    // Métodos para consultar a vida se precisar em outro código
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMaxima() => vidaMaxima;
}