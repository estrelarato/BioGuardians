using UnityEngine;
using UnityEngine.SceneManagement;

public class CarregadorGUI : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Inicializar()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
        VerificarECarregarGUI(SceneManager.GetActiveScene());
    }

    static void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        if (modo == LoadSceneMode.Single)
        {
            VerificarECarregarGUI(cena);
        }
    }

    static void VerificarECarregarGUI(Scene cena)
    {
        // Só carrega a GUIad se a cena estiver dentro da pasta "Cenas/Fases/"
        // Ignora automaticamente _Boot, Dicionario, Menu, Opções e qualquer outra pasta
        if (!cena.path.Contains("Cenas/Fases/")) return;

        if (!SceneManager.GetSceneByName("GUIad").isLoaded)
        {
            SceneManager.LoadScene("GUIad", LoadSceneMode.Additive);
        }
    }
}