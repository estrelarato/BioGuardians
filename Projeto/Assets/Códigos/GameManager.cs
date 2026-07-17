using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Interface de Vitória")]
    public GameObject painelVitoria;

    private bool checarVitoria = false;

    void Start()
    {
        Time.timeScale = 1f;
        if (painelVitoria != null)
        {
            painelVitoria.SetActive(false);
        }
    }

    void Update()
    {
        // Só começa a checar os inimigos quando o Gerenciador de Ondas mandar
        if (checarVitoria)
        {
            ChecarCondicaoVitoria();
        }
    }

    // Essa função será chamada pelo GerenciadorOndas quando a última onda terminar
    public void AtivarChecagemVitoria()
    {
        checarVitoria = true;
    }

    void ChecarCondicaoVitoria()
    {
        Inimigo inimigoRestante = FindFirstObjectByType<Inimigo>();

        if (inimigoRestante == null)
        {
            checarVitoria = false; // Para de checar
            GanharJogo();
        }
    }

    void GanharJogo()
    {
        Debug.Log("Parabéns! Você defendeu a base de todas as ondas!");
        
        if (painelVitoria != null)
        {
            painelVitoria.SetActive(true);
        }

        Time.timeScale = 0f; // Pausa o jogo
    }
}
