using UnityEngine;

public class TropaNeutrofila : MonoBehaviour
{
    [Header("Configurações do Minion")]
    public float velocidade = 4f;
    public float dano = 25f;
    public float raioExplosao = 1.5f;
    public float raioDeteccao = 5f;
    public float distanciaParaExplodir = 0.5f;

    [Header("Componentes e Efeitos")]
    public Animator animadorTropa;
    public SpriteRenderer spriteRenderer;
    public GameObject efeitoExplosaoPrefab;

    [Header("Tempo da Animação")]
    [Tooltip("Duração em segundos da animação de explosão")]
    public float tempoAnimacaoExplosao = 0.5f;

    private Transform alvoAtual;
    private bool estaExplodindo = false;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (estaExplodindo) return;

        BuscarAlvoMaisProximo();

        if (alvoAtual != null)
        {
            AtualizarAnimacao(true);

            // Move-se diretamente para o alvo
            transform.position = Vector2.MoveTowards(
                transform.position, 
                alvoAtual.position, 
                velocidade * Time.deltaTime
            );

            // Flip do sprite baseado no movimento horizontal
            float direcaoX = alvoAtual.position.x - transform.position.x;
            AtualizarFlipSprite(direcaoX);

            float distancia = Vector2.Distance(transform.position, alvoAtual.position);
            if (distancia <= distanciaParaExplodir)
            {
                Explodir();
            }
        }
        else
        {
            AtualizarAnimacao(false);
        }
    }

    void AtualizarFlipSprite(float direcaoX)
    {
        if (Mathf.Abs(direcaoX) > 0.05f && spriteRenderer != null)
        {
            spriteRenderer.flipX = (direcaoX < 0);
        }
    }

    void AtualizarAnimacao(bool estaMovendo)
    {
        if (animadorTropa != null)
        {
            animadorTropa.SetBool("EstaAndando", estaMovendo);
        }
    }

    void BuscarAlvoMaisProximo()
    {
        Inimigo[] inimigos = FindObjectsOfType<Inimigo>();
        float menorDistancia = Mathf.Infinity;
        Transform melhorAlvo = null;

        foreach (Inimigo inimigo in inimigos)
        {
            float dist = Vector2.Distance(transform.position, inimigo.transform.position);
            if (dist < menorDistancia && dist <= raioDeteccao)
            {
                menorDistancia = dist;
                melhorAlvo = inimigo.transform;
            }
        }

        alvoAtual = melhorAlvo;
    }

    void Explodir()
    {
        estaExplodindo = true;

        // Desativa colisor para não causar múltiplos impactos
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animadorTropa != null)
        {
            animadorTropa.SetTrigger("Explodir");
        }

        if (efeitoExplosaoPrefab != null)
        {
            Instantiate(efeitoExplosaoPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] acertos = Physics2D.OverlapCircleAll(transform.position, raioExplosao);
        foreach (Collider2D colisor in acertos)
        {
            Inimigo inimigo = colisor.GetComponent<Inimigo>();
            if (inimigo != null)
            {
                inimigo.SendMessage("ReceberDano", dano, SendMessageOptions.DontRequireReceiver);
            }
        }

        // Aguarda a animação terminar antes de destruir o minion
        Destroy(gameObject, tempoAnimacaoExplosao);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}