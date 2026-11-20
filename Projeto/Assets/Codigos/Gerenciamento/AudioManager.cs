using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }

    [Header("Mixer Principal")]
    public AudioMixer mainMixer;

    [Header("Fontes de Áudio Globais")]
    public AudioSource fonteMusica;
    public AudioSource fonteSFX;

    private const string CHAVE_MUSICA = "PrefVolumeMusica";
    private const string CHAVE_SFX = "PrefVolumeSFX";

    public float volumeMusicaAtual { get; private set; } = 0.75f;
    public float volumeSFXAtual { get; private set; } = 0.75f;

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
        // Carrega os volumes salvos (padrão 0.75 se for a primeira vez)
        SetVolumeMusica(PlayerPrefs.GetFloat(CHAVE_MUSICA, 0.75f));
        SetVolumeSFX(PlayerPrefs.GetFloat(CHAVE_SFX, 0.75f));
    }

    public void SetVolumeMusica(float valorSlider)
    {
        volumeMusicaAtual = valorSlider;
        // Converte escala linear do Slider (0.0001 a 1) para decibéis (-80dB a 0dB)
        float db = Mathf.Log10(Mathf.Clamp(valorSlider, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat("VolumeMusica", db);

        PlayerPrefs.SetFloat(CHAVE_MUSICA, valorSlider);
        PlayerPrefs.Save();
    }

    public void SetVolumeSFX(float valorSlider)
    {
        volumeSFXAtual = valorSlider;
        float db = Mathf.Log10(Mathf.Clamp(valorSlider, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat("VolumeSFX", db);

        PlayerPrefs.SetFloat(CHAVE_SFX, valorSlider);
        PlayerPrefs.Save();
    }

    // Atalho prático para tocar sons de clique, torres ou feitiços de qualquer script
    public void TocarSFX(AudioClip clip)
    {
        if (clip != null && fonteSFX != null)
        {
            fonteSFX.PlayOneShot(clip);
        }
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