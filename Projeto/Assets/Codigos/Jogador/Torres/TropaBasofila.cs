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

    [Header("Área de Limite da Torre")]
    public Vector3 posicaoTorreOrigem;
    public float raioAreaTorre = 5.0f;

    [Header("Tipo de Dano")]
    public TipoDano tipoDano = TipoDano.Fagocito;

    [Header("Componentes 2D e Animação")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Parâmetros do Animator")]
    public string nomeBoolAndando = "estaAndando";
    public string nomeTriggerAtaque = "Atacar";

    private Transform pontoRally;
    private Inimigo alvoInimigo;
    private float tempoUltimoAtaque;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void ConfigurarTorreERally(Vector3 origemTorre, float raioMaximo, Transform rally)
    {
        posicaoTorreOrigem = origemTorre;
        raioAreaTorre = raioMaximo;
        pontoRally = rally;
    }

    void Update()
    {
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
                ExecutarAtaque();
                estaAndando = false;
            }
        }
        else if (pontoRally != null)
        {
            Vector3 destinoValidado = RestringirPosicaoAArea(pontoRally.position);
            float distRally = Vector2.Distance(transform.position, destinoValidado);
            
            if (distRally > 0.2f)
            {
                MoverPara(destinoValidado);
                estaAndando = true;
            }
            else
            {
                estaAndando = false;
            }
        }
        else
        {
            estaAndando = false;
        }

        if (animator != null)
        {
            animator.SetBool(nomeBoolAndando, estaAndando);
        }
    }

    void MoverPara(Vector3 destino)
    {
        Vector3 destinoValidado = RestringirPosicaoAArea(destino);

        transform.position = Vector2.MoveTowards(
            transform.position, 
            destinoValidado, 
            velocidade * Time.deltaTime
        );

        float direcaoX = destinoValidado.x - transform.position.x;
        if (Mathf.Abs(direcaoX) > 0.05f && spriteRenderer != null)
        {
            spriteRenderer.flipX = (direcaoX < 0);
        }
    }

    Vector3 RestringirPosicaoAArea(Vector3 posicaoDesejada)
    {
        Vector3 vetorDistancia = posicaoDesejada - posicaoTorreOrigem;
        if (vetorDistancia.magnitude > raioAreaTorre)
        {
            return posicaoTorreOrigem + vetorDistancia.normalized * raioAreaTorre;
        }
        return posicaoDesejada;
    }

    void BuscarInimigo()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, raioVisao);
        float menorDistancia = Mathf.Infinity;
        Inimigo inimigoProximo = null;

        foreach (Collider2D hit in hits)
        {
            Inimigo inig = hit.GetComponent<Inimigo>();
            if (inig != null)
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

            if (animator != null)
            {
                animator.SetTrigger(nomeTriggerAtaque);
            }

            Vector3 direcaoEmpurrao = ObterDirecaoOpostaAoMovimento(alvoInimigo);

            alvoInimigo.ReceberDano(danoAtaque, tipoDano);
            alvoInimigo.AplicarKnockback(direcaoEmpurrao * forcaKnockback);
        }
    }

    Vector3 ObterDirecaoOpostaAoMovimento(Inimigo inimigo)
    {
        if (inimigo.pontosCaminho != null && inimigo.pontosCaminho.Length > 0)
        {
            Transform waypointDestino = ObterPontoAtualInimigo(inimigo);
            if (waypointDestino != null)
            {
                Vector3 direcaoIndo = (waypointDestino.position - inimigo.transform.position).normalized;
                return -direcaoIndo;
            }
        }

        return (inimigo.transform.position - transform.position).normalized;
    }

    Transform ObterPontoAtualInimigo(Inimigo inimigo)
    {
        System.Reflection.FieldInfo campoIndice = typeof(Inimigo).GetField("indicePontoAtual", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
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