using UnityEngine;

public class TocarMusicaCena : MonoBehaviour
{
    [Header("Música desta Cena")]
    public AudioClip musicaDaCena;

    void Start()
    {
        if (AudioManager.Instancia != null && musicaDaCena != null)
        {
            AudioManager.Instancia.TocarMusica(musicaDaCena);
        }
    }
}