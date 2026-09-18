using UnityEngine;

public class VidaDoJogador : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 20;
    private int vidaAtual;

    void Start()
    {
        vidaAtual = vidaMaxima;
        Debug.Log("Jogo Iniciado! Vida da Base: " + vidaAtual);
    }

    public void TomarDano(int quantidade)
    {
        vidaAtual -= quantidade;
        Debug.Log("A base sofreu dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            
            // --- NOVO: Avisa o GameManager para disparar a derrota ---
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PerderJogo();
            }
        }
    }
}