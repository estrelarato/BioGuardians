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
    public int recompensaMoedas = 20; // Quanto este inimigo dá de ouro ao morrer

    private bool estaVivo = true;
    private Transform[] caminhos;
    private int indiceCaminhoAtual = 0;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    void Update()
    {
        if (!estaVivo) return;

        if (caminhos != null && indiceCaminhoAtual < caminhos.Length)
        {
            Transform alvoAtual = caminhos[indiceCaminhoAtual];
            transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

            if (Vector2.Distance(transform.position, alvoAtual.position) < 0.1f)
            {
                indiceCaminhoAtual++;
            }
        }
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

        if (vidaAtual <= 0)
        {
            estaVivo = false;

            // --- NOVO: Dá moedas ao jogador apenas se morrer para as torres ---
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

            // Se o inimigo fugir e passar da base, ele apenas se destrói (não dá moedas)
            Destroy(gameObject);
        }
    }
}