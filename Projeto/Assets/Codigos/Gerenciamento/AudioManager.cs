using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }

    [Header("Mixer Principal")]
    public AudioMixer mainMixer;

    [Header("Fontes de Áudio Globais (4 AudioSources no _BootSystem)")]
    public AudioSource fonteMusica;
    public AudioSource fonteTorres;
    public AudioSource fonteInimigos;
    public AudioSource fonteUI;

    // Chaves de Save
    private const string CHAVE_GERAL = "PrefVolGeral";
    private const string CHAVE_MUSICA = "PrefVolMusica";
    private const string CHAVE_TORRES = "PrefVolTorres";
    private const string CHAVE_INIMIGOS = "PrefVolInimigos";
    private const string CHAVE_UI = "PrefVolUI";

    // Valores atuais (0.0001 a 1)
    public float volGeral { get; private set; } = 1f;
    public float volMusica { get; private set; } = 0.75f;
    public float volTorres { get; private set; } = 0.8f;
    public float volInimigos { get; private set; } = 0.8f;
    public float volUI { get; private set; } = 0.8f;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetVolumeGeral(PlayerPrefs.GetFloat(CHAVE_GERAL, 1f));
        SetVolumeMusica(PlayerPrefs.GetFloat(CHAVE_MUSICA, 0.75f));
        SetVolumeTorres(PlayerPrefs.GetFloat(CHAVE_TORRES, 0.8f));
        SetVolumeInimigos(PlayerPrefs.GetFloat(CHAVE_INIMIGOS, 0.8f));
        SetVolumeUI(PlayerPrefs.GetFloat(CHAVE_UI, 0.8f));
    }

    private void AplicarVolumeMixer(string parametroExposto, float valorSlider, string chaveSave)
    {
        float db = Mathf.Log10(Mathf.Clamp(valorSlider, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat(parametroExposto, db);
        PlayerPrefs.SetFloat(chaveSave, valorSlider);
        PlayerPrefs.Save();
    }

    public void SetVolumeGeral(float valor)
    {
        volGeral = valor;
        AplicarVolumeMixer("VolumeGeral", valor, CHAVE_GERAL);
    }

    public void SetVolumeMusica(float valor)
    {
        volMusica = valor;
        AplicarVolumeMixer("VolumeMusica", valor, CHAVE_MUSICA);
    }

    public void SetVolumeTorres(float valor)
    {
        volTorres = valor;
        AplicarVolumeMixer("VolumeTorres", valor, CHAVE_TORRES);
    }

    public void SetVolumeInimigos(float valor)
    {
        volInimigos = valor;
        AplicarVolumeMixer("VolumeInimigos", valor, CHAVE_INIMIGOS);
    }

    public void SetVolumeUI(float valor)
    {
        volUI = valor;
        AplicarVolumeMixer("VolumeUI", valor, CHAVE_UI);
    }

    // --- Atalhos para tocar os sons de qualquer script do jogo ---
    public void TocarSomTorre(AudioClip clip)
    {
        if (clip != null && fonteTorres != null) fonteTorres.PlayOneShot(clip);
    }

    public void TocarSomInimigo(AudioClip clip)
    {
        if (clip != null && fonteInimigos != null) fonteInimigos.PlayOneShot(clip);
    }

    public void TocarSomUI(AudioClip clip)
    {
        if (clip != null && fonteUI != null) fonteUI.PlayOneShot(clip);
    }

    public void TocarMusica(AudioClip musica)
    {
        if (musica == null || fonteMusica == null) return;
        if (fonteMusica.clip == musica && fonteMusica.isPlaying) return;

        fonteMusica.clip = musica;
        fonteMusica.loop = true;
        fonteMusica.Play();
    }
}