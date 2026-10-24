using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaInimigo : MonoBehaviour
{
    public Slider sliderVidasRestantes;
    public Inimigo inimigo;

    [SerializeField]
    private float vidasRestantes = 0;

    void Start()
    {
        // Se não tiver arrastado o Slider no Inspector, tenta pegar no próprio objeto
        if (sliderVidasRestantes == null)
        {
            sliderVidasRestantes = GetComponent<Slider>();
        }

        // Se não tiver arrastado o Inimigo, busca no pai
        if (inimigo == null)
        {
            inimigo = GetComponentInParent<Inimigo>();
        }

        if (inimigo != null && sliderVidasRestantes != null)
        {
            sliderVidasRestantes.minValue = 0;
            sliderVidasRestantes.maxValue = inimigo.vidaMaxima;
            sliderVidasRestantes.value = inimigo.vidaAtual;
        }
    }

    void Update()
    {
        if (sliderVidasRestantes != null && inimigo != null)
        {
            vidasRestantes = inimigo.vidaAtual;
            sliderVidasRestantes.value = vidasRestantes;
        }
    }

    /// <summary>
    /// Atualiza o Slider imediatamente quando chamada pelo Inimigo ao tomar dano.
    /// </summary>
    public void AtualizarBarra()
    {
        if (sliderVidasRestantes != null && inimigo != null)
        {
            vidasRestantes = inimigo.vidaAtual;
            sliderVidasRestantes.value = vidasRestantes;
        }
    }
}