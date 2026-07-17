using UnityEngine;

public class VidaDoJogador : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 20;
    private int vidaAtual;

    [Header("Interface de Game Over")]
    public GameObject painelGameOver; // <-- Arraste o painel preto para cá no Inspector

    void Start()
    {
        // Garante que o tempo do jogo está normal ao iniciar
        Time.timeScale = 1f; 
        
        vidaAtual = vidaMaxima;
        
        // Garante que a tela de Game Over comece escondida
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
        
        Debug.Log("Jogo Iniciado! Vida da Base: " + vidaAtual);
    }

    public void TomarDano(int quantidade)
    {
        vidaAtual -= quantidade;
        Debug.Log("A base sofreu dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            Derrota();
        }
    }

    void Derrota()
    {
        Debug.Log("Game Over! A base foi destruída.");
        
        // Ativa a tela preta de Game Over
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
        }

        // Pausa o jogo (congela inimigos e torres)
        Time.timeScale = 0f; 
    }
}