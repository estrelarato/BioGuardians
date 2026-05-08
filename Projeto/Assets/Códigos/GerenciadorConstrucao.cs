using UnityEngine;

public class GerenciadorConstrucao : MonoBehaviour
{
    public static GerenciadorConstrucao instancia;

    public GameObject torreSelecionada;

    void Awake()
    {
        instancia = this;
    }

    public void SelecionarTorre(GameObject torre)
    {
        torreSelecionada = torre;
    }
}