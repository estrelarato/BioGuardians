using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos")]
    public float vidaMaxima = 100f;
    public float vidaAtual;

    public float velocidade = 2f;
    public float dano = 10f;
    public float defesa = 0f;

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
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Tenta encontrar o script da barra de vida nos filhos caso não esteja arrastado no Inspector
        if (barraDeVida == null)
        {
            barraDeVida = GetComponentInChildren<BarraDeVidaInimigo>();
        }

        posicaoAnteriorX = transform.position.x;
        
        // Atualiza a barra assim que nasce
        if (barraDeVida != null)
        {
            barraDeVida.AtualizarBarra();
        }
    }

    protected virtual void Update()
    {
        if (!estaVivo) return;

        if (caminhos != null && indiceCaminhoAtual < caminhos.Length)
        {
            Transform alvoAtual = caminhos[indiceCaminhoAtual];
            
            // A movimentação usa diretamente a variável 'velocidade', 
            // portanto responderá instantaneamente quando o 'EfeitoLento' alterar o valor!
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

        if (Mathf.Abs(deslocamentoX) > 0.001f)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = (deslocamentoX < 0);
            }
        }

        posicaoAnteriorX = transform.position.x;
    }

    public void DefinirCaminho(Transform[] pontosDeCaminho)
    {
        caminhos = pontosDeCaminho;
        indiceCaminhoAtual = 0;
    }

    public void ReceberDano(float quantidade)
    {
        if (!estaVivo) return;

        // Correção do cálculo de dano (subtraindo a defesa)
        float danoFinal = Mathf.Max(quantidade - defesa, 0);
        vidaAtual -= danoFinal;

        // Avisa o script da barra para recalcular o preenchimento visual
        if (barraDeVida != null)
        {
            barraDeVida.AtualizarBarra();
        }

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