using UnityEngine;

public class TropaEosinofila : MonoBehaviour
{
    [Header("Atributos de Movimento")]
    [Tooltip("Velocidade de caminhada mais suave e calma")]
    public float velocidade = 2.0f;

    [Header("Distâncias Táticas e Comportamento")]
    [Tooltip("Distância que a Eosinófila fica ATRÁS da Basófila em relação ao inimigo")]
    public float distanciaIdealAtrasBasofila = 1.8f;

    [Tooltip("Distância mínima que a posição ideal precisa mudar para ela se mexer (evita que ande à toa)")]
    public float toleranciaMovimento = 0.8f;

    public float alcanceTiro = 5.0f;
    public float taxaDisparo = 0.35f;

    [Header("Área de Limite da Torre")]
    public Vector3 posicaoTorreOrigem;
    public float raioAreaTorre = 5.0f;

    [Header("Tipo de Dano")]
    public TipoDano tipoDano = TipoDano.Enzimatico;

    [Header("Ataque e Prefabs")]
    public GameObject prefabProjetil;
    public Transform canoEsquerdo;
    public Transform canoDireito;

    [Header("Componentes 2D e Animação")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Parâmetros do Animator")]
    public string nomeBoolAndando = "estaAndando";
    public string nomeTriggerAtaque = "Atacar";

    private TropaBasofila basofilaAlvo;
    private float tempoUltimoTiro;
    private bool usarCanoEsquerdo = true;
    
    private Vector3 ultimaPosicaoDestino;
    private bool estaEmMovimento = false;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
        ultimaPosicaoDestino = transform.position;
    }

    public void ConfigurarTorre(Vector3 origemTorre, float raioMaximo)
    {
        posicaoTorreOrigem = origemTorre;
        raioAreaTorre = raioMaximo;
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
        bool estaAndando = false;

        if (basofilaAlvo != null)
        {
            Transform alvoInimigo = basofilaAlvo.ObterAlvoAtual();
            
            if (alvoInimigo != null)
            {
                Vector3 posicaoIdeal = CalcularPosicaoTaticaGarantida(alvoInimigo);
                estaAndando = GerenciarMovimentoTranquilo(posicaoIdeal);
                
                TentarAtacarInimigo(alvoInimigo);
            }
            else
            {
                // Sem inimigos: reset e fica parada no lugar
                estaEmMovimento = false;
                estaAndando = false;
            }
        }
        else
        {
            BuscarNovaBasofila();
            estaEmMovimento = false;
            estaAndando = false;
        }

        if (animator != null)
        {
            animator.SetBool(nomeBoolAndando, estaAndando);
        }
    }

    Vector3 CalcularPosicaoTaticaGarantida(Transform inimigoTransform)
    {
        Vector3 posInimigo = inimigoTransform.position;
        Vector3 posBasofila = basofilaAlvo.transform.position;

        // Vetor que vai do Inimigo em direção à Basófila
        Vector3 direcaoInimigoParaBasofila = (posBasofila - posInimigo).normalized;

        // Mantém a hierarquia rigorosa: Eosinófila -> Basófila -> Inimigo
        Vector3 posicaoAtrasDaBasofila = posBasofila + (direcaoInimigoParaBasofila * distanciaIdealAtrasBasofila);

        return RestringirPosicaoAArea(posicaoAtrasDaBasofila);
    }

    bool GerenciarMovimentoTranquilo(Vector3 destinoIdeal)
    {
        float distAteNovoDestino = Vector2.Distance(transform.position, destinoIdeal);

        // Só começa a andar se o alvo sair significativamente da zona de conforto
        if (!estaEmMovimento && distAteNovoDestino > toleranciaMovimento)
        {
            estaEmMovimento = true;
            ultimaPosicaoDestino = destinoIdeal;
        }

        // Se já estiver andando, só para quando chegar bem perto do destino final
        if (estaEmMovimento)
        {
            float distRestante = Vector2.Distance(transform.position, ultimaPosicaoDestino);

            if (distRestante > 0.15f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    ultimaPosicaoDestino, 
                    velocidade * Time.deltaTime
                );

                float direcaoX = ultimaPosicaoDestino.x - transform.position.x;
                if (Mathf.Abs(direcaoX) > 0.05f && spriteRenderer != null)
                {
                    spriteRenderer.flipX = (direcaoX < 0);
                }

                return true; // Está andando
            }
            else
            {
                // Chegou na posição tranquila: para de andar!
                estaEmMovimento = false;
            }
        }

        return false; // Parada atirando de boas
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

    void BuscarNovaBasofila()
    {
        TropaBasofila encontrada = FindFirstObjectByType<TropaBasofila>();
        if (encontrada != null)
        {
            ConfigurarAlvoBasofila(encontrada);
        }
    }

    void TentarAtacarInimigo(Transform alvoTransform)
    {
        if (alvoTransform == null) return;

        Inimigo inimigoAlvo = alvoTransform.GetComponent<Inimigo>();
        if (inimigoAlvo == null) return;

        float dist = Vector2.Distance(transform.position, inimigoAlvo.transform.position);
        if (dist <= alcanceTiro)
        {
            float direcaoX = inimigoAlvo.transform.position.x - transform.position.x;
            if (Mathf.Abs(direcaoX) > 0.05f && spriteRenderer != null)
            {
                spriteRenderer.flipX = (direcaoX < 0);
            }

            if (Time.time >= tempoUltimoTiro + taxaDisparo)
            {
                tempoUltimoTiro = Time.time;

                if (animator != null)
                {
                    animator.SetTrigger(nomeTriggerAtaque);
                }

                DispararProjetil(inimigoAlvo);
            }
        }
    }

    void DispararProjetil(Inimigo inimigoAlvo)
    {
        Transform pontoSaida = usarCanoEsquerdo ? canoEsquerdo : canoDireito;
        usarCanoEsquerdo = !usarCanoEsquerdo;

        if (pontoSaida == null) pontoSaida = transform;

        GameObject proj = Instantiate(prefabProjetil, pontoSaida.position, pontoSaida.rotation);
        
        Projetil scriptProj = proj.GetComponent<Projetil>();
        if (scriptProj != null)
        {
            scriptProj.DefinirAlvo(inimigoAlvo);
            scriptProj.tipoDano = tipoDano;
            return;
        }

        Magia scriptMagia = proj.GetComponent<Magia>();
        if (scriptMagia != null)
        {
            scriptMagia.DefinirAlvo(inimigoAlvo);
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