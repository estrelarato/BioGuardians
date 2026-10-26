using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarDeCena : MonoBehaviour
{
    // Troca a cena pelo NOME (Ex: "FaseTestes" ou "MenuPrincipal")
    public void CarregarCenaPorNome(string nomeDaCena)
    {
        Time.timeScale = 1f; // Garante que o jogo não continue pausado se veio de um Game Over
        SceneManager.LoadScene(nomeDaCena);
    }

    // Troca a cena pelo ÍNDICE no Build Settings (Ex: 0, 1, 2)
    public void CarregarCenaPorIndice(int indiceDaCena)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(indiceDaCena);
    }
}