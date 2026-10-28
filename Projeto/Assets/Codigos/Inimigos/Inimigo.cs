using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos Básicos")]
    public float vidaMaxima = 100f;
    public float vidaAtual;
    public float velocidade = 2f;
    public float dano = 10f;

    [Header("Resistências Biológicas (0 = 0% | 1 = 100%)")]
    [Range(0f, 1f)] 
    [Tooltip("Reduz o dano de ataques Fagocíticos/Mecânicos")]
    public float resistenciaFagocitica = 0f;

    [Range(0f, 1f)] 
    [Tooltip("Reduz o dano de ataques Enzimáticos/Químicos")]
    public float resistenciaEnzimatica = 0f;

    [Header("Economia")]
    public int recompensaMoedas = 20;

    [Header("Gerenciador de Barra de Vida")]
    public BarraDeVidaInimigo barraDeVida;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    protected bool estaVivo = true;
    protected Transform[] caminhos;
    protected int indiceCaminhoAtual = 0;
    protected float posicaoAnteriorX;

    void Awake()
    {
        vidaAtual = vidaMaxima;
    }

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (barraDeVida == null)
            barraDeVida = GetComponentInChildren<BarraDeVidaInimigo>();

        posicaoAnteriorX = transform.position.x;
        
        if (barraDeVida != null)
            barraDeVida.AtualizarBarra();
    }

    protected virtual void Update()
    {
        if (!estaVivo) return;

        if (caminhos != null && indiceCaminhoAtual < caminhos.Length)
        {
            Transform alvoAtual = caminhos[indiceCaminhoAtual];
            transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

            AtualizarFlipSprite();

            if (Vector2.Distance(transform.position, alvoAtual.position) < 0.1f)
            {
                indiceCaminhoAtual++;
            }
        }
    }

    void AtualizarFlipSprite()
    {
        float deslocamentoX = transform.position.x - posicaoAnteriorX;
        if (Mathf.Abs(deslocamentoX) > 0.001f && spriteRenderer != null)
        {
            spriteRenderer.flipX = (deslocamentoX < 0);
        }
        posicaoAnteriorX = transform.position.x;
    }

    public void DefinirCaminho(Transform[] pontosDeCaminho)
    {
        caminhos = pontosDeCaminho;
        indiceCaminhoAtual = 0;
    }

    public void DefinirCaminho(Transform[] pontosDeCaminho, int pontoInicial)
    {
        caminhos = pontosDeCaminho;
        indiceCaminhoAtual = pontoInicial;
    }

    /// <summary>
    /// Processa o dano baseado no tipo (Fagocítico, Enzimático ou Verdadeiro)
    /// </summary>
    public void ReceberDano(float quantidade, TipoDano tipoDano = TipoDano.Fagocito)
    {
        if (!estaVivo) return;

        float danoCalculado = quantidade;

        switch (tipoDano)
        {
            case TipoDano.Fagocito:
                danoCalculado = quantidade * (1f - resistenciaFagocitica);
                break;

            case TipoDano.Enzimatico:
                danoCalculado = quantidade * (1f - resistenciaEnzimatica);
                break;

            case TipoDano.Verdadeiro:
                danoCalculado = quantidade; // Ignora qualquer tipo de resistência
                break;
        }

        danoCalculado = Mathf.Max(danoCalculado, 0);
        vidaAtual -= danoCalculado;

        if (barraDeVida != null)
            barraDeVida.AtualizarBarra();

        if (vidaAtual <= 0)
        {
            estaVivo = false;
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.GanharDinheiro(recompensaMoedas);
            }
            Morrer();
        }
    }

    void Morrer()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FimDoCaminho"))
        {
            VidaDoJogador sistemaVida = FindFirstObjectByType<VidaDoJogador>();
            if (sistemaVida != null)
            {
                sistemaVida.TomarDano((int)dano);
            }
            Destroy(gameObject);
        }
    }
}