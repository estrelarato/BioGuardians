using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos Básicos")]
    public float vidaMaxima = 100f;
    public float vidaAtual;
    public float velocidade = 2f;
    public float dano = 10f;

    [Header("Resistências Biológicas (0.0 = 0% a 1.0 = 100%)")]
    [Range(0f, 1f)] public float resistenciaFagocito = 0f;  // Reduz dano de Fagócito (ex: Cápsula espessa)
    [Range(0f, 1f)] public float resistenciaEnzimatica = 0f; // Reduz dano Enzimático (ex: Inativação de enzimas)

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
    /// Recebe o dano calculando a resistência correspondente (Fagócito ou Enzimático)
    /// </summary>
    public void ReceberDano(float quantidade, TipoDano tipoDano = TipoDano.Fagocito)
    {
        if (!estaVivo) return;

        float danoCalculado = quantidade;

        switch (tipoDano)
        {
            case TipoDano.Fagocito:
                danoCalculado = quantidade * (1f - resistenciaFagocito);
                break;
            case TipoDano.Enzimatico:
                danoCalculado = quantidade * (1f - resistenciaEnzimatica);
                break;
            case TipoDano.Verdadeiro:
                danoCalculado = quantidade;
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