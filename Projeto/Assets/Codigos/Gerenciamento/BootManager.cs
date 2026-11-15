using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    [Header("Configurações de Inicialização")]
    public string nomeCenaMenu = "Menu";
    public float tempoMinimoSplash = 1.0f;

    void Start()
    {
        StartCoroutine(InicializarJogo());
    }

    private IEnumerator InicializarJogo()
    {
        Time.timeScale = 1f;
        yield return null;

        if (tempoMinimoSplash > 0)
        {
            yield return new WaitForSeconds(tempoMinimoSplash);
        }

        SceneManager.LoadSceneAsync(nomeCenaMenu);
    }
}