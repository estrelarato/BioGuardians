using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreAdipocita : TorreBase
{
    [Header("Configurações Gerais")]
    public int custo = 50;
    public bool emConstrucao = true;
    public float tempoConstrucao = 3f;

    [Header("Geração de ATP")]
    public int quantidadeATP = 10;
    public float tempoEntreGeracoes = 5f;

    [Header("Feedback Visual")]
    public SpriteRenderer spriteRenderer; // Arraste o SpriteRenderer aqui pelo Inspector
    public Color corPiscada = Color.white;
    public float duracaoPiscada = 0.15f;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;    // Base ou estrutura da torre
    public Animator animadorGerador;  // Topo, núcleo ou detalhes visuais

    [Header("Configurações de Animação")]
    public float duracaoAnimacaoGeracao = 0.5f;

    private float timerConstrucao;
    private float timerGeracao;
    private GameManager gameManager;
    private Coroutine corrotinaGeracao;
    private Coroutine corrotinaPiscada;
    private Color corOriginalSprite;

    void Awake()
    {
        // Debug para confirmar se a torre está a ser instanciada na cena ao clicar no botão
        Debug.Log("[TorreAdipocita] Objeto criado com sucesso na cena!");
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("[ERRO] GameManager não foi encontrado na cena!");
        }

        // Tenta obter automaticamente o SpriteRenderer se não tiver sido atribuído no Inspector
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            corOriginalSprite = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning("[AVISO] Nenhum SpriteRenderer foi encontrado na TorreAdipocita!");
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

        // Dispara o pulso visual nos animadores
        if (corrotinaGeracao != null)
        {
            StopCoroutine(corrotinaGeracao);
        }
        corrotinaGeracao = StartCoroutine(ControladorBoolGeracao());

        // Dispara a piscadinha rápida no sprite
        if (corrotinaPiscada != null)
        {
            StopCoroutine(corrotinaPiscada);
        }
        corrotinaPiscada = StartCoroutine(EfeitoPiscada());
    }

    private IEnumerator ControladorBoolGeracao()
    {
        if (animadorTorre != null) animadorTorre.SetBool("Gerando", true);
        if (animadorGerador != null) animadorGerador.SetBool("Gerando", true);

        yield return new WaitForSeconds(duracaoAnimacaoGeracao);

        if (animadorTorre != null) animadorTorre.SetBool("Gerando", false);
        if (animadorGerador != null) animadorGerador.SetBool("Gerando", false);
    }

    private IEnumerator EfeitoPiscada()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corPiscada;
            yield return new WaitForSeconds(duracaoPiscada);
            spriteRenderer.color = corOriginalSprite;
        }
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