using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos")]
    public float vidaMaxima = 100f;
    public float vidaAtual;

    public float velocidade = 2f;
    public float dano = 10f;
    public float defesa = 0f;

    private bool estaVivo = true;

    // --- VARIÁVEIS DO CAMINHO (ADICIONADAS) ---
    private Transform[] caminhos;
    private int indiceCaminhoAtual = 0;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    // --- LÓGICA DE MOVIMENTAÇÃO (ADICIONADA) ---
    void Update()
    {
        if (!estaVivo) return;

        // Se o caminho foi definido, move o inimigo em direção ao ponto atual
        if (caminhos != null && indiceCaminhoAtual < caminhos.Length)
        {
            Transform alvoAtual = caminhos[indiceCaminhoAtual];
            
            // Move o inimigo até o ponto de caminho
            transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

            // Se chegou muito perto do ponto atual, avança para o próximo ponto
            if (Vector2.Distance(transform.position, alvoAtual.position) < 0.1f)
            {
                indiceCaminhoAtual++;
            }
        }
    }

    // --- FUNÇÃO QUE O GERENCIADOR CHAMA (ADICIONADA) ---
    // Ela DEVE ser 'public' para o GerenciadorOndas conseguir usá-la
    public void DefinirCaminho(Transform[] pontosDeCaminho)
    {
        caminhos = pontosDeCaminho;
        indiceCaminhoAtual = 0; // Começa do primeiro ponto
    }

    public void ReceberDano(float quantidade)
    {
        if (!estaVivo) return;

        float danoFinal = Mathf.Max(quantidade - defesa, 0);
        vidaAtual -= danoFinal;

        if (vidaAtual <= 0)
        {
            estaVivo = false;
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