using UnityEngine;
using UnityEngine.UI;

public class ControleVolumeUI : MonoBehaviour
{
    public Slider sliderMusica;
    public Slider sliderSFX;

    void OnEnable()
    {
        if (AudioManager.Instancia == null) return;

        // Remove eventos temporariamente para não disparar ao sincronizar o visual
        if (sliderMusica != null)
        {
            sliderMusica.onValueChanged.RemoveAllListeners();
            sliderMusica.minValue = 0.0001f;
            sliderMusica.maxValue = 1f;
            sliderMusica.value = AudioManager.Instancia.volumeMusicaAtual;
            sliderMusica.onValueChanged.AddListener(AudioManager.Instancia.SetVolumeMusica);
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.RemoveAllListeners();
            sliderSFX.minValue = 0.0001f;
            sliderSFX.maxValue = 1f;
            sliderSFX.value = AudioManager.Instancia.volumeSFXAtual;
            sliderSFX.onValueChanged.AddListener(AudioManager.Instancia.SetVolumeSFX);
        }
    }
}