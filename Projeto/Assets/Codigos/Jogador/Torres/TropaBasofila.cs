using System.Collections;
using UnityEngine;

public class TropaBasofila : MonoBehaviour
{
    [Header("Atributos de Combate")]
    public float velocidade = 3.5f;
    public float danoAtaque = 20f;
    public float alcanceAtaque = 1.2f;
    public float intervaloAtaque = 1.2f;
    public float forcaKnockback = 2.0f;
    public float raioVisao = 6.0f;
    public LayerMask layerInimigos;

    [Header("Área de Limite da Torre")]
    public Vector2 posicaoTorreOrigem;
    public float raioAreaTorre = 5.0f;

    [Header("Tipo de Dano")]
    public TipoDano tipoDano = TipoDano.Fagocito;

    [Header("Componentes 2D e Animação")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    [Tooltip("Marque se a arte original do sprite já nasce olhando para a esquerda")]
    public bool spriteOlhaParaEsquerdaPorPadrao = false;
    [Tooltip("Duração total que a tropa fica parada executando o golpe")]
    public float duracaoAnimacaoAtaque = 0.35f;
    [Range(0f, 1f), Tooltip("Momento da animação em que o dano é aplicado (0.5 = metade)")]
    public float momentoImpactoGolpe = 0.5f;

    [Header("Parâmetros do Animator")]
    public string nomeBoolAndando = "Andando";
    public string nomeTriggerAtacar = "Atacar";

    private Transform pontoRally;
    private Inimigo alvoInimigo;
    private float tempoUltimoAtaque;
    private bool atacando = false;
    private Coroutine corrotinaAtaque;

    private int hashAndando;
    private int hashAtacar;
    private readonly Collider2D[] bufferInimigos = new Collider2D[16];
    private float proximoTempoBusca;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        hashAndando = Animator.StringToHash(nomeBoolAndando);
        hashAtacar = Animator.StringToHash(nomeTriggerAtacar);
    }

    public void ConfigurarTorreERally(Vector3 origemTorre, float raioMaximo, Transform rally)
    {
        posicaoTorreOrigem = origemTorre;
        raioAreaTorre = raioMaximo;
        pontoRally = rally;
    }

    void Update()
    {
        // Enquanto ataca, fica parada e continua olhando para o inimigo
        if (atacando)
        {
            AtualizarAnimatorAndando(false);
            if (alvoInimigo != null)
            {
                OlharPara(alvoInimigo.transform.position);
            }
            return;
        }

        BuscarInimigo();

        bool estaAndando = false;

        if (alvoInimigo != null)
        {
            float distancia = Vector2.Distance(transform.position, alvoInimigo.transform.position);

            if (distancia > alcanceAtaque)
            {
                MoverPara(alvoInimigo.transform.position);
                estaAndando = true;
            }
            else
            {
                OlharPara(alvoInimigo.transform.position);
                ExecutarAtaque();
                estaAndando = false;
            }
        }
        else if (pontoRally != null)
        {
            Vector2 destinoValidado = RestringirPosicaoAArea(pontoRally.position);
            float distRally = Vector2.Distance(transform.position, destinoValidado);

            if (distRally > 0.2f)
            {
                MoverPara(destinoValidado);
                estaAndando = true;
            }
        }

        AtualizarAnimatorAndando(estaAndando);
    }

    void AtualizarAnimatorAndando(bool andando)
    {
        if (animator == null) return;
        animator.SetBool(hashAndando, andando);
    }

    void MoverPara(Vector2 destino)
    {
        Vector2 destinoValidado = RestringirPosicaoAArea(destino);

        // Vira para a direção do movimento antes de deslocar
        OlharPara(destinoValidado);

        transform.position = Vector2.MoveTowards(
            transform.position,
            destinoValidado,
            velocidade * Time.deltaTime
        );
    }

    void OlharPara(Vector2 alvoPosicao)
    {
        float direcaoX = alvoPosicao.x - transform.position.x;

        // Evita tremida (flip rápido) quando o alvo está exatamente na mesma linha vertical
        if (Mathf.Abs(direcaoX) <= 0.02f) return;

        bool indoParaEsquerda = direcaoX < 0f;
        bool deveInverter = spriteOlhaParaEsquerdaPorPadrao ? !indoParaEsquerda : indoParaEsquerda;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = deveInverter;
        }
    }

    Vector2 RestringirPosicaoAArea(Vector2 posicaoDesejada)
    {
        Vector2 vetorDistancia = posicaoDesejada - posicaoTorreOrigem;
        if (vetorDistancia.magnitude > raioAreaTorre)
        {
            return posicaoTorreOrigem + vetorDistancia.normalized * raioAreaTorre;
        }
        return posicaoDesejada;
    }

    void BuscarInimigo()
    {
        if (alvoInimigo != null)
        {
            float distTorreAlvo = Vector2.Distance(alvoInimigo.transform.position, posicaoTorreOrigem);
            float distVisaoAlvo = Vector2.Distance(transform.position, alvoInimigo.transform.position);

            if (distTorreAlvo > raioAreaTorre || distVisaoAlvo > raioVisao)
            {
                alvoInimigo = null;
            }
        }

        if (Time.time < proximoTempoBusca) return;
        proximoTempoBusca = Time.time + 0.15f;

        int quantidade = layerInimigos != 0
            ? Physics2D.OverlapCircleNonAlloc(transform.position, raioVisao, bufferInimigos, layerInimigos)
            : Physics2D.OverlapCircleNonAlloc(transform.position, raioVisao, bufferInimigos);

        float menorDistancia = Mathf.Infinity;
        Inimigo inimigoProximo = null;

        for (int i = 0; i < quantidade; i++)
        {
            if (bufferInimigos[i].TryGetComponent(out Inimigo inig))
            {
                float distTorre = Vector2.Distance(inig.transform.position, posicaoTorreOrigem);
                if (distTorre <= raioAreaTorre)
                {
                    float dist = Vector2.Distance(transform.position, inig.transform.position);
                    if (dist < menorDistancia)
                    {
                        menorDistancia = dist;
                        inimigoProximo = inig;
                    }
                }
            }
        }

        alvoInimigo = inimigoProximo;
    }

    void ExecutarAtaque()
    {
        if (Time.time >= tempoUltimoAtaque + intervaloAtaque && alvoInimigo != null)
        {
            tempoUltimoAtaque = Time.time;

            // Garante que olha para o alvo no início do ataque
            OlharPara(alvoInimigo.transform.position);

            if (corrotinaAtaque != null) StopCoroutine(corrotinaAtaque);
            corrotinaAtaque = StartCoroutine(RotinaAnimacaoAtaque());
        }
    }

    private IEnumerator RotinaAnimacaoAtaque()
    {
        atacando = true;
        AtualizarAnimatorAndando(false);

        if (animator != null)
        {
            animator.ResetTrigger(hashAtacar);
            animator.SetTrigger(hashAtacar);
        }

        float tempoAntesImpacto = duracaoAnimacaoAtaque * momentoImpactoGolpe;
        yield return new WaitForSeconds(tempoAntesImpacto);

        if (alvoInimigo != null)
        {
            OlharPara(alvoInimigo.transform.position);
            Vector3 direcaoEmpurrao = ObterDirecaoOpostaAoMovimento(alvoInimigo);
            alvoInimigo.ReceberDano(danoAtaque, tipoDano);
            alvoInimigo.AplicarKnockback(direcaoEmpurrao * forcaKnockback);
        }

        float tempoRestante = duracaoAnimacaoAtaque - tempoAntesImpacto;
        if (tempoRestante > 0f)
        {
            yield return new WaitForSeconds(tempoRestante);
        }

        atacando = false;
    }

    Vector3 ObterDirecaoOpostaAoMovimento(Inimigo inimigo)
    {
        if (inimigo.pontosCaminho != null && inimigo.pontosCaminho.Length > 0)
        {
            Transform waypointDestino = ObterPontoAtualInimigo(inimigo);
            if (waypointDestino != null)
            {
                Vector3 direcaoIndo = waypointDestino.position - inimigo.transform.position;
                if (direcaoIndo.sqrMagnitude > 0.001f)
                {
                    return -direcaoIndo.normalized;
                }
            }
        }

        return (inimigo.transform.position - transform.position).normalized;
    }

    Transform ObterPontoAtualInimigo(Inimigo inimigo)
    {
        var campoIndice = typeof(Inimigo).GetField(
            "indicePontoAtual",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (campoIndice != null)
        {
            int idx = (int)campoIndice.GetValue(inimigo);
            if (idx >= 0 && idx < inimigo.pontosCaminho.Length)
            {
                return inimigo.pontosCaminho[idx];
            }
        }
        return null;
    }

    public Transform ObterAlvoAtual()
    {
        return alvoInimigo != null ? alvoInimigo.transform : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, raioVisao);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(posicaoTorreOrigem, raioAreaTorre);
    }
}