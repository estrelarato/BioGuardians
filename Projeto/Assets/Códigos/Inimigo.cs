using UnityEngine;
using UnityEngine.UI;

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

    [Header("Interface (Barra de Vida)")]
    public Image barraDeVidaPreenchimento;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    private bool estaVivo = true;
    private Transform[] caminhos;
    private int indiceCaminhoAtual = 0;
    private float posicaoAnteriorX;

    void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarBarraDeVida();

        // Tenta obter o SpriteRenderer automaticamente caso não esteja atribuído no Inspector
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        posicaoAnteriorX = transform.position.x;
    }

    void Update()
    {
        if (!estaVivo) return;

        if (caminhos != null && indiceCaminhoAtual < caminhos.Length)
        {
            Transform alvoAtual = caminhos[indiceCaminhoAtual];
            transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

            // --- Lógica de Flip do Sprite ---
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

        // Margem de segurança para evitar flicker se estiver parado/alinhado no eixo X
        if (Mathf.Abs(deslocamentoX) > 0.001f)
        {
            if (spriteRenderer != null)
            {
                // Inverte se estiver indo para a esquerda (assumindo sprite original virado para a direita)
                spriteRenderer.flipX = (deslocamentoX < 0);
            }
            else
            {
                // Alternativa invertendo a escala no eixo X
                Vector3 escala = transform.localScale;
                escala.x = deslocamentoX < 0 ? -Mathf.Abs(escala.x) : Mathf.Abs(escala.x);
                transform.localScale = escala;
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

        float danoFinal = Mathf.Max(quantidade - defesa, 0);
        vidaAtual -= danoFinal;

        AtualizarBarraDeVida();

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

    void AtualizarBarraDeVida()
    {
        if (barraDeVidaPreenchimento != null)
        {
            barraDeVidaPreenchimento.fillAmount = vidaAtual / vidaMaxima;
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