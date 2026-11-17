using UnityEngine;
using UnityEngine.SceneManagement;

public class CarregadorDeFase : MonoBehaviour
{
    public string nomeCenaGUI = "GUIad";

    public void IniciarFase(string nomeCenaDaFase)
    {
        // 1. Carrega a fase principal (substitui o mapa)
        SceneManager.LoadScene(nomeCenaDaFase, LoadSceneMode.Single);

        // 2. Carrega a cena da GUI por cima da fase
        SceneManager.LoadScene(nomeCenaGUI, LoadSceneMode.Additive);
    }
}