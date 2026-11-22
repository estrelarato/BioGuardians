using UnityEngine;
using UnityEngine.UI;

public class ConectorSlidersAudio : MonoBehaviour
{
    [Header("Sliders da Tela de Opções")]
    public Slider sliderGeral;
    public Slider sliderMusica;
    public Slider sliderTorres;
    public Slider sliderInimigos;
    public Slider sliderUI;

    void OnEnable()
    {
        if (AudioManager.Instancia == null) return;

        // 1. Posiciona os sliders nos valores salvos sem disparar eventos duplicados
        ConfigurarSlider(sliderGeral, "Geral", AudioManager.Instancia.SetVolumeGeral);
        ConfigurarSlider(sliderMusica, "Musica", AudioManager.Instancia.SetVolumeMusica);
        ConfigurarSlider(sliderTorres, "Torres", AudioManager.Instancia.SetVolumeTorres);
        ConfigurarSlider(sliderInimigos, "Inimigos", AudioManager.Instancia.SetVolumeInimigos);
        ConfigurarSlider(sliderUI, "UI", AudioManager.Instancia.SetVolumeUI);
    }

    private void ConfigurarSlider(Slider slider, string nomeCanal, UnityEngine.Events.UnityAction<float> acao)
    {
        if (slider == null) return;

        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(AudioManager.Instancia.GetVolumeSalvo(nomeCanal));

        slider.onValueChanged.RemoveListener(acao);
        slider.onValueChanged.AddListener(acao);
    }
}