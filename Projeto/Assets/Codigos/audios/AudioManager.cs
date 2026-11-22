using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }

    [Header("Mixer Principal")]
    public AudioMixer mainMixer;

    [Header("Fontes de Áudio")]
    public AudioSource fonteMusica;
    public AudioSource fonteUI;
    public AudioSource fonteInimigos;
    public AudioSource fonteTorres;

    // Propriedades públicas usadas pelo ControleVolumeUI.cs
    public float volGeral { get; private set; } = 1f;
    public float volMusica { get; private set; } = 1f;
    public float volTorres { get; private set; } = 1f;
    public float volInimigos { get; private set; } = 1f;
    public float volUI { get; private set; } = 1f;

    // Chaves para salvar as preferências do jogador
    private const string CHAVE_GERAL = "Vol_Geral";
    private const string CHAVE_MUSICA = "Vol_Musica";
    private const string CHAVE_TORRES = "Vol_Torres";
    private const string CHAVE_INIMIGOS = "Vol_Inimigos";
    private const string CHAVE_UI = "Vol_UI";

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CarregarVolumesSalvos();
    }

    public void CarregarVolumesSalvos()
    {
        SetVolumeGeral(PlayerPrefs.GetFloat(CHAVE_GERAL, 1f));
        SetVolumeMusica(PlayerPrefs.GetFloat(CHAVE_MUSICA, 1f));
        SetVolumeTorres(PlayerPrefs.GetFloat(CHAVE_TORRES, 1f));
        SetVolumeInimigos(PlayerPrefs.GetFloat(CHAVE_INIMIGOS, 1f));
        SetVolumeUI(PlayerPrefs.GetFloat(CHAVE_UI, 1f));
    }

    private float ConverterParaDecibeis(float valorSlider)
    {
        float valorSeguro = Mathf.Clamp(valorSlider, 0.0001f, 1f);
        return Mathf.Log10(valorSeguro) * 20f;
    }

    public void SetVolumeGeral(float valor)
    {
        volGeral = valor;
        if (mainMixer != null) mainMixer.SetFloat("VolumeGeral", ConverterParaDecibeis(valor));
        PlayerPrefs.SetFloat(CHAVE_GERAL, valor);
    }

    public void SetVolumeMusica(float valor)
    {
        volMusica = valor;
        if (mainMixer != null) mainMixer.SetFloat("VolumeMusica", ConverterParaDecibeis(valor));
        PlayerPrefs.SetFloat(CHAVE_MUSICA, valor);
    }

    public void SetVolumeTorres(float valor)
    {
        volTorres = valor;
        if (mainMixer != null) mainMixer.SetFloat("VolumeTorres", ConverterParaDecibeis(valor));
        PlayerPrefs.SetFloat(CHAVE_TORRES, valor);
    }

    public void SetVolumeInimigos(float valor)
    {
        volInimigos = valor;
        if (mainMixer != null) mainMixer.SetFloat("VolumeInimigos", ConverterParaDecibeis(valor));
        PlayerPrefs.SetFloat(CHAVE_INIMIGOS, valor);
    }

    public void SetVolumeUI(float valor)
    {
        volUI = valor;
        if (mainMixer != null) mainMixer.SetFloat("VolumeUI", ConverterParaDecibeis(valor));
        PlayerPrefs.SetFloat(CHAVE_UI, valor);
    }

    public float GetVolumeSalvo(string canal)
    {
        switch (canal)
        {
            case "Geral": return volGeral;
            case "Musica": return volMusica;
            case "Torres": return volTorres;
            case "Inimigos": return volInimigos;
            case "UI": return volUI;
            default: return 1f;
        }
    }

    // --- Métodos para tocar cada categoria de som ---

    public void TocarMusica(AudioClip musica)
    {
        if (fonteMusica == null || musica == null) return;
        if (fonteMusica.clip == musica && fonteMusica.isPlaying) return;

        fonteMusica.clip = musica;
        fonteMusica.loop = true;
        fonteMusica.Play();
    }

    public void TocarSomUI(AudioClip som)
    {
        if (fonteUI == null || som == null) return;
        fonteUI.PlayOneShot(som);
    }

    public void TocarSomInimigo(AudioClip som)
    {
        if (fonteInimigos == null || som == null) return;
        fonteInimigos.PlayOneShot(som);
    }

    public void TocarSomTorre(AudioClip som)
    {
        if (fonteTorres == null || som == null) return;
        fonteTorres.PlayOneShot(som);
    }
}