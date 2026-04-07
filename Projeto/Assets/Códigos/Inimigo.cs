using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos")]
    public float vidaMaxima = 100f;
    public float vidaAtual;

    public float velocidade = 2f;
    public float dano = 10f;
    public float defesa = 0f;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(float quantidade)
    {
        float danoFinal = Mathf.Max(quantidade - defesa, 0);
        vidaAtual -= danoFinal;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        Destroy(gameObject);
    }
}