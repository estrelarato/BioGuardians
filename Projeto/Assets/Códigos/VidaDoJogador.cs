using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VidaDoJogador : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 20;
    private int vidaAtual;

    [Header("Interface de Usuário (Opcional)")]
    public TextMeshProUGUI textoVida;
    public Image barraDeVida;

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
            textoVida.text = vidaAtual + " / " + vidaMaxima;
        }

        if (barraDeVida != null)
        {
            barraDeVida.fillAmount = (float)vidaAtual / vidaMaxima;
        }
    }
}