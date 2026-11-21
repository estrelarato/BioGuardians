using UnityEngine;
using UnityEngine.UI;

public class ControleVolumeUI : MonoBehaviour
{
    [Header("Sliders de Volume")]
    public Slider sliderGeral;
    public Slider sliderMusica;
    public Slider sliderTorres;
    public Slider sliderInimigos;
    public Slider sliderUI;

    void OnEnable()
    {
        if (AudioManager.Instancia == null) return;

        ConfigurarSlider(sliderGeral, AudioManager.Instancia.volGeral, AudioManager.Instancia.SetVolumeGeral);
        ConfigurarSlider(sliderMusica, AudioManager.Instancia.volMusica, AudioManager.Instancia.SetVolumeMusica);
        ConfigurarSlider(sliderTorres, AudioManager.Instancia.volTorres, AudioManager.Instancia.SetVolumeTorres);
        ConfigurarSlider(sliderInimigos, AudioManager.Instancia.volInimigos, AudioManager.Instancia.SetVolumeInimigos);
        ConfigurarSlider(sliderUI, AudioManager.Instancia.volUI, AudioManager.Instancia.SetVolumeUI);
    }

    private void ConfigurarSlider(Slider slider, float valorAtual, UnityEngine.Events.UnityAction<float> acao)
    {
        if (slider == null) return;

        slider.onValueChanged.RemoveAllListeners();
        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.value = valorAtual;
        slider.onValueChanged.AddListener(acao);
    }
}