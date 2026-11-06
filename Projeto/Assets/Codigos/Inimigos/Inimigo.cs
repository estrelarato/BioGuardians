using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos de Vida e Movimento 2D")]
    public float vidaMaxima = 100f;
    public float vidaAtual;
    public float velocidade = 3.5f;

    [Header("Resistências ao Tipo de Dano")]
    [Tooltip("Coloque valores entre 0 (imune) e 1 (dano total). Valores acima de 1 aumentam o dano.")]
    public float resistenciaFagocito = 1.0f;
    public float resistenciaEnzimatico = 1.0f;
    [Tooltip("Dano Verdadeiro ignora resistências (deixe em 1.0f)")]
    public float resistenciaVerdadeiro = 1.0f;

    [Header("Caminho 2D (Waypoints)")]
    public Transform[] pontosCaminho;
    private int indicePontoAtual = 0;

    [Header("Componentes 2D")]
    public SpriteRenderer spriteRenderer;

    private bool estaEmKnockback = false;

    void Awake()
    {
        vidaAtual = vidaMaxima;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (estaEmKnockback) return;

        // O inimigo segue puramente o caminho
        MoverPeloCaminho();
    }

    public void DefinirCaminho(Transform[] caminho)
    {
        pontosCaminho = caminho;
        indicePontoAtual = 0;
    }

    void MoverPeloCaminho()
    {
        if (pontosCaminho == null || pontosCaminho.Length == 0 || indicePontoAtual >= pontosCaminho.Length) return;

        Transform destino = pontosCaminho[indicePontoAtual];
        MoverPara(destino.position);

        if (Vector2.Distance(transform.position, destino.position) <= 0.1f)
        {
            indicePontoAtual++;
        }
    }

    void MoverPara(Vector3 posicaoDestino)
    {
        transform.position = Vector2.MoveTowards(
            transform.position, 
            posicaoDestino, 
            velocidade * Time.deltaTime
        );

        float direcaoX = posicaoDestino.x - transform.position.x;
        if (Mathf.Abs(direcaoX) > 0.05f && spriteRenderer != null)
        {
            spriteRenderer.flipX = (direcaoX < 0);
        }
    }

    // Sobrecarga de dano simples (padrão)
    public void ReceberDano(float quantidade)
    {
        ReceberDano(quantidade, TipoDano.Fagocito);
    }

    // Processa o dano aplicando os multiplicadores de resistência
    public void ReceberDano(float quantidade, TipoDano tipo)
    {
        float danoFinal = quantidade;

        switch (tipo)
        {
            case TipoDano.Fagocito:
                danoFinal *= resistenciaFagocito;
                break;
            case TipoDano.Enzimatico:
                danoFinal *= resistenciaEnzimatico;
                break;
            case TipoDano.Verdadeiro:
                danoFinal *= resistenciaVerdadeiro;
                break;
        }

        vidaAtual -= danoFinal;

        BarraDeVidaInimigo barra = GetComponentInChildren<BarraDeVidaInimigo>();
        if (barra != null) barra.AtualizarBarra();

        if (vidaAtual <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void AplicarKnockback(Vector3 forca)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(RoutineKnockback(forca));
    }

    private IEnumerator RoutineKnockback(Vector3 forca)
    {
        estaEmKnockback = true;

        float tempo = 0f;
        float duracao = 0.2f;
        Vector3 inicio = transform.position;
        Vector3 final = inicio + forca;

        while (tempo < duracao)
        {
            transform.position = Vector3.Lerp(inicio, final, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        estaEmKnockback = false;
    }
}