using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreGeradoraATP : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public int custo = 50;
    public bool emConstrucao = true;
    public float tempoConstrucao = 3f;

    [Header("Geração de ATP")]
    public int quantidadeATP = 10;
    public float tempoEntreGeracoes = 5f;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;    // Base ou estrutura da torre
    public Animator animadorGerador;  // Topo, núcleo ou detalhes visuais

    [Header("Configurações de Animação")]
    public float duracaoAnimacaoGeracao = 0.5f;

    [Header("Efeito Visual de Piscada (Flash)")]
    public SpriteRenderer spriteRenderer; // Arraste o SpriteRenderer aqui pelo Inspector
    public Color corPiscada = Color.white;  // Cor que ele vai piscar (ex: Color.white ou com Alfa transparente)
    public float duracaoPiscada = 0.1f;    // Duração rápida do piscar (ex: 100ms)

    private float timerConstrucao;
    private float timerGeracao;
    private GameManager gameManager;
    private Coroutine corrotinaGeracao;
    private Color corOriginalSprite;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("[ERRO] GameManager não foi encontrado na cena!");
        }

        // Caso não tenha atribuído o SpriteRenderer no Inspector, tenta pegar no próprio GameObject
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            corOriginalSprite = spriteRenderer.color;
        }

        timerConstrucao = tempoConstrucao;
        timerGeracao = tempoEntreGeracoes;
    }

    void Update()
    {
        if (gameManager == null) return;

        // FASE 1: Aguarda o término da construção
        if (emConstrucao)
        {
            ProcessarConstrucao();
            AtualizarAnimacoesEstado(emConstrucao: true);
            return;
        }

        AtualizarAnimacoesEstado(emConstrucao: false);

        // FASE 2: Processa o ciclo de produção de ATP
        ProcessarGeracaoATP();
    }

    void ProcessarConstrucao()
    {
        timerConstrucao -= Time.deltaTime;

        if (timerConstrucao <= 0f)
        {
            emConstrucao = false;
        }
    }

    void ProcessarGeracaoATP()
    {
        timerGeracao -= Time.deltaTime;

        if (timerGeracao <= 0f)
        {
            GerarATP();
            timerGeracao = tempoEntreGeracoes; // Reseta o ciclo
        }
    }

    void GerarATP()
    {
        // Adiciona ATP ao GameManager
        gameManager.GanharDinheiro(quantidadeATP);

        // Dispara o pulso visual nos animadores e o piscar do sprite
        if (corrotinaGeracao != null)
        {
            StopCoroutine(corrotinaGeracao);
        }
        corrotinaGeracao = StartCoroutine(ControladorBoolGeracao());
    }

    private IEnumerator ControladorBoolGeracao()
    {
        // Ativa os parâmetros do Animator
        if (animadorTorre != null) animadorTorre.SetBool("Gerando", true);
        if (animadorGerador != null) animadorGerador.SetBool("Gerando", true);

        // Efeito de Piscada Rápida
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corPiscada;
        }

        yield return new WaitForSeconds(duracaoPiscada);

        // Volta a cor original do sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginalSprite;
        }

        // Aguarda o restante do tempo da animação padrão (se a duração da animação for maior)
        float tempoRestante = duracaoAnimacaoGeracao - duracaoPiscada;
        if (tempoRestante > 0f)
        {
            yield return new WaitForSeconds(tempoRestante);
        }

        // Desativa os parâmetros do Animator
        if (animadorTorre != null) animadorTorre.SetBool("Gerando", false);
        if (animadorGerador != null) animadorGerador.SetBool("Gerando", false);
    }

    void AtualizarAnimacoesEstado(bool emConstrucao)
    {
        if (animadorTorre != null)
            animadorTorre.SetBool("EmConstrucao", emConstrucao);

        if (animadorGerador != null)
            animadorGerador.SetBool("EmConstrucao", emConstrucao);
    }

    // Utilitários de consulta para UI ou feedbacks visuais
    public float GetProgressoConstrucao()
    {
        if (!emConstrucao) return 1f;
        return Mathf.Clamp01(1f - (timerConstrucao / tempoConstrucao));
    }
}