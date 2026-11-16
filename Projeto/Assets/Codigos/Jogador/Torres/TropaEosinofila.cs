using System.Collections;
using UnityEngine;

public class TropaEosinofila : MonoBehaviour
{
    [Header("Atributos de Movimento e Alcance")]
    [Tooltip("Velocidade de caminhada mais suave e calma")]
    public float velocidade = 2.0f;
    public float alcanceTiro = 5.0f;
    public float taxaDisparo = 0.8f;
    public LayerMask layerInimigos;

    [Header("Área de Limite da Torre")]
    public Vector2 posicaoTorreOrigem;
    public float raioAreaTorre = 5.0f;
    [Tooltip("Distância máxima que ela fica parada em relação à torre quando não há inimigos")]
    public float distanciaRepousoTorre = 1.2f;

    [Header("Tipo de Dano")]
    public TipoDano tipoDano = TipoDano.Enzimatico;

    [Header("Ataque e Prefabs")]
    public GameObject prefabProjetil;
    [Tooltip("Ponto de saída da Arma 1 (primeiro tiro da animação)")]
    public Transform canoArma1;
    [Tooltip("Ponto de saída da Arma 2 (segundo tiro da animação)")]
    public Transform canoArma2;

    [Header("Componentes 2D e Animação")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    [Tooltip("Opcional: objeto pai dos sprites e dos canos para inverter tudo junto via escala X")]
    public Transform objetoVisual;
    [Tooltip("Marque se a arte original do sprite já nasce olhando para a esquerda")]
    public bool spriteOlhaParaEsquerdaPorPadrao = false;
    [Tooltip("Duração total da animação de tiro (tempo que fica parada disparando as duas armas)")]
    public float duracaoAnimacaoAtaque = 0.6f;

    [Header("Parâmetros do Animator (Bools)")]
    public string nomeBoolAndando = "Andando";
    public string nomeBoolAtirando = "Atirando";

    private TropaBasofila basofilaAlvo;
    private Inimigo inimigoAtual;
    private Vector2 pontoRepousoPertoDaTorre;
    private float tempoUltimoTiro;
    private bool estaAtirando = false;
    private Coroutine corrotinaAtaque;

    private int hashAndando;
    private int hashAtirando;
    private readonly Collider2D[] bufferInimigos = new Collider2D[16];
    private float proximoTempoBusca;
    private float escalaBaseX = 1f;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (objetoVisual != null)
        {
            escalaBaseX = Mathf.Abs(objetoVisual.localScale.x);
            if (escalaBaseX < 0.001f) escalaBaseX = 1f;
        }

        hashAndando = Animator.StringToHash(nomeBoolAndando);
        hashAtirando = Animator.StringToHash(nomeBoolAtirando);

        pontoRepousoPertoDaTorre = transform.position;
    }

    public void ConfigurarTorre(Vector3 origemTorre, float raioMaximo)
    {
        posicaoTorreOrigem = origemTorre;
        raioAreaTorre = raioMaximo;

        Vector2 direcaoAtual = ((Vector2)transform.position - posicaoTorreOrigem).normalized;
        if (direcaoAtual.sqrMagnitude < 0.01f) direcaoAtual = Vector2.down;

        pontoRepousoPertoDaTorre = RestringirPosicaoAArea(posicaoTorreOrigem + direcaoAtual * distanciaRepousoTorre);
    }

    public void ConfigurarAlvoBasofila(TropaBasofila baso)
    {
        basofilaAlvo = baso;
        if (baso != null)
        {
            ConfigurarTorre(baso.posicaoTorreOrigem, baso.raioAreaTorre);
        }
    }

    void Update()
    {
        if (estaAtirando)
        {
            AtualizarAnimator(false, true);
            if (inimigoAtual != null)
            {
                OlharPara(inimigoAtual.transform.position);
            }
            return;
        }

        AtualizarAlvo();

        bool estaAndando = false;

        if (inimigoAtual != null)
        {
            float distanciaParaInimigo = Vector2.Distance(transform.position, inimigoAtual.transform.position);

            if (distanciaParaInimigo > alcanceTiro)
            {
                Vector2 posicaoAnterior = transform.position;
                MoverPara(inimigoAtual.transform.position);

                if (Vector2.Distance(posicaoAnterior, transform.position) > 0.001f)
                {
                    estaAndando = true;
                }
                else
                {
                    OlharPara(inimigoAtual.transform.position);
                }
            }
            else
            {
                estaAndando = false;
                OlharPara(inimigoAtual.transform.position);
                IniciarSequenciaDeTiro();
            }
        }
        else
        {
            float distParaRepouso = Vector2.Distance(transform.position, pontoRepousoPertoDaTorre);
            if (distParaRepouso > 0.15f)
            {
                MoverPara(pontoRepousoPertoDaTorre);
                estaAndando = true;
            }
        }

        AtualizarAnimator(estaAndando, estaAtirando);
    }

    void AtualizarAlvo()
    {
        if (basofilaAlvo != null)
        {
            Transform alvoBaso = basofilaAlvo.ObterAlvoAtual();
            if (alvoBaso != null && alvoBaso.TryGetComponent(out Inimigo inigBaso))
            {
                if (Vector2.Distance(inigBaso.transform.position, posicaoTorreOrigem) <= raioAreaTorre)
                {
                    inimigoAtual = inigBaso;
                    return;
                }
            }
        }
        else
        {
            TropaBasofila encontrada = FindFirstObjectByType<TropaBasofila>();
            if (encontrada != null) ConfigurarAlvoBasofila(encontrada);
        }

        if (inimigoAtual != null)
        {
            if (Vector2.Distance(inimigoAtual.transform.position, posicaoTorreOrigem) > raioAreaTorre)
            {
                inimigoAtual = null;
            }
        }

        if (Time.time < proximoTempoBusca) return;
        proximoTempoBusca = Time.time + 0.15f;

        int quantidade = layerInimigos != 0
            ? Physics2D.OverlapCircleNonAlloc(posicaoTorreOrigem, raioAreaTorre, bufferInimigos, layerInimigos)
            : Physics2D.OverlapCircleNonAlloc(posicaoTorreOrigem, raioAreaTorre, bufferInimigos);

        float menorDistancia = Mathf.Infinity;
        Inimigo maisProximo = null;

        for (int i = 0; i < quantidade; i++)
        {
            if (bufferInimigos[i].TryGetComponent(out Inimigo inig))
            {
                float dist = Vector2.Distance(transform.position, inig.transform.position);
                if (dist < menorDistancia)
                {
                    menorDistancia = dist;
                    maisProximo = inig;
                }
            }
        }

        inimigoAtual = maisProximo;
    }

    void MoverPara(Vector2 destino)
    {
        Vector2 destinoValidado = RestringirPosicaoAArea(destino);
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
        if (Mathf.Abs(direcaoX) <= 0.02f) return;

        bool indoParaEsquerda = direcaoX < 0f;
        bool deveInverter = spriteOlhaParaEsquerdaPorPadrao ? !indoParaEsquerda : indoParaEsquerda;

        if (objetoVisual != null)
        {
            Vector3 escala = objetoVisual.localScale;
            escala.x = deveInverter ? -escalaBaseX : escalaBaseX;
            objetoVisual.localScale = escala;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.flipX = deveInverter;
        }
    }

    void AtualizarAnimator(bool andando, bool atirando)
    {
        if (animator == null) return;

        animator.SetBool(hashAndando, andando);
        animator.SetBool(hashAtirando, atirando);
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

    void IniciarSequenciaDeTiro()
    {
        if (inimigoAtual == null) return;

        if (Time.time >= tempoUltimoTiro + taxaDisparo)
        {
            tempoUltimoTiro = Time.time;

            if (corrotinaAtaque != null) StopCoroutine(corrotinaAtaque);
            corrotinaAtaque = StartCoroutine(RotinaAnimacaoAtaque());
        }
    }

    private IEnumerator RotinaAnimacaoAtaque()
    {
        estaAtirando = true;
        AtualizarAnimator(false, true);

        yield return new WaitForSeconds(duracaoAnimacaoAtaque);

        estaAtirando = false;
    }

    // =========================================================================
    // MÉTODOS CHAMADOS PELOS ANIMATION EVENTS NA TIMELINE DA ANIMAÇÃO
    // =========================================================================

    public void DispararArma1()
    {
        ExecutarDisparoNoCano(canoArma1);
    }

    public void DispararArma2()
    {
        ExecutarDisparoNoCano(canoArma2);
    }

    private void ExecutarDisparoNoCano(Transform pontoSaida)
    {
        // Se o alvo morreu entre o tiro 1 e o tiro 2, tenta pegar outro alvo válido na hora
        if (inimigoAtual == null)
        {
            AtualizarAlvo();
            if (inimigoAtual == null) return;
        }

        OlharPara(inimigoAtual.transform.position);

        if (pontoSaida == null) pontoSaida = transform;

        GameObject proj = Instantiate(prefabProjetil, pontoSaida.position, pontoSaida.rotation);

        Projetil scriptProj = proj.GetComponent<Projetil>();
        if (scriptProj != null)
        {
            scriptProj.DefinirAlvo(inimigoAtual);
            scriptProj.tipoDano = tipoDano;
            return;
        }

        Magia scriptMagia = proj.GetComponent<Magia>();
        if (scriptMagia != null)
        {
            scriptMagia.DefinirAlvo(inimigoAtual);
            scriptMagia.tipoDano = tipoDano;
            scriptMagia.LiberarTiro();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcanceTiro);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(posicaoTorreOrigem, raioAreaTorre);
    }
}