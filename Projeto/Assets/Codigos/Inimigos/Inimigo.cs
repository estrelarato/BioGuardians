using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos de Vida e Movimento 2D")]
    public float vidaMaxima = 100f;
    public float vidaAtual;
    public float velocidade = 3.5f;

    [Header("Ataque na Base / Fim do Caminho")]
    public int danoAoEncostar = 1;
    public string tagAlvo = "FimDoCaminho";
    private bool jaCausouDano = false;

    [Header("Resistências ao Tipo de Dano")]
    [Tooltip("Coloque valores entre 0 (imune) e 1 (dano total). Valores acima de 1 aumentam o dano.")]
    public float resistenciaFagocito = 1.0f;
    public float resistenciaEnzimatico = 1.0f;
    [Tooltip("Dano Verdadeiro ignora resistências (deixe em 1.0f)")]
    public float resistenciaVerdadeiro = 1.0f;

    [Header("Caminho 2D (Waypoints)")]
    public Transform[] pontosCaminho;
    private int indicePontoAtual = 0;

    // Propriedade para permitir que scripts externos leiam o waypoint atual
    public int IndicePontoAtual => indicePontoAtual;

    [Header("Componentes 2D e Animação")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    [Tooltip("Nome do parâmetro Trigger de morte no Animator")]
    public string nomeTriggerMorte = "Morrer";
    [Tooltip("Tempo que a animação de morte leva antes de começar a sumir")]
    public float duracaoAnimacaoMorte = 0.6f;
    [Tooltip("Tempo que o sprite leva sumindo aos poucos (Fade Out)")]
    public float duracaoFadeOut = 0.8f;

    private bool estaEmKnockback = false;
    private bool estaMorto = false;
    private int hashMorrer;

    public bool EstaMorto => estaMorto;

    void Awake()
    {
        vidaAtual = vidaMaxima;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        hashMorrer = Animator.StringToHash(nomeTriggerMorte);
    }

    void Update()
    {
        if (estaMorto || estaEmKnockback) return;

        // O inimigo segue puramente o caminho
        MoverPeloCaminho();
    }

    // Sobrecarga: Permite definir o caminho começando de um waypoint específico
    public void DefinirCaminho(Transform[] caminho, int indiceInicial = 0)
    {
        pontosCaminho = caminho;
        indicePontoAtual = indiceInicial;
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

    // Detecta quando o inimigo entra na área marcada com a Tag FimDoCaminho
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (estaMorto || jaCausouDano) return;

        if (other.CompareTag(tagAlvo))
        {
            jaCausouDano = true;

            VidaDoJogador vidaJogador = other.GetComponent<VidaDoJogador>();
            if (vidaJogador == null)
            {
                vidaJogador = FindFirstObjectByType<VidaDoJogador>();
            }

            if (vidaJogador != null)
            {
                vidaJogador.TomarDano(danoAoEncostar);
            }
            else
            {
                Debug.LogWarning("VidaDoJogador não encontrado na cena!");
            }

            Destroy(gameObject);
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
        if (estaMorto) return;

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
            vidaAtual = 0;
            Morrer();
        }
    }

    void Morrer()
    {
        if (estaMorto) return;
        estaMorto = true;

        // 1. Interrompe qualquer movimento ou knockback em andamento
        StopAllCoroutines();
        estaEmKnockback = false;

        // 2. Desativa todos os colisores para que torres e tropas parem de detectá-lo
        Collider2D[] colisores = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colisores)
        {
            col.enabled = false;
        }

        // 3. Desativa a barra de vida visual
        BarraDeVidaInimigo barra = GetComponentInChildren<BarraDeVidaInimigo>();
        if (barra != null)
        {
            barra.gameObject.SetActive(false);
        }

        // 4. Inicia a sequência de animação de morte + fade out
        StartCoroutine(RotinaMorte());
    }

    private IEnumerator RotinaMorte()
    {
        if (animator != null)
        {
            // Garante que o Animator está ativo e atualizando
            animator.enabled = true;

            if (!string.IsNullOrEmpty(nomeTriggerMorte))
            {
                animator.SetTrigger(hashMorrer);
            }
        }
        else
        {
            Debug.LogWarning($"[Inimigo] Animator não encontrado em {gameObject.name}!");
        }

        // Aguarda a animação de morte tocar por completo
        if (duracaoAnimacaoMorte > 0f)
        {
            yield return new WaitForSeconds(duracaoAnimacaoMorte);
        }

        // Faz o sprite desaparecer aos poucos (Fade Out)
        if (spriteRenderer != null && duracaoFadeOut > 0f)
        {
            Color corInicial = spriteRenderer.color;
            float tempoDecorrido = 0f;

            while (tempoDecorrido < duracaoFadeOut)
            {
                tempoDecorrido += Time.deltaTime;
                float novoAlpha = Mathf.Lerp(corInicial.a, 0f, tempoDecorrido / duracaoFadeOut);
                spriteRenderer.color = new Color(corInicial.r, corInicial.g, corInicial.b, novoAlpha);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    public void AplicarKnockback(Vector3 forca)
    {
        if (estaMorto || !gameObject.activeInHierarchy) return;
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