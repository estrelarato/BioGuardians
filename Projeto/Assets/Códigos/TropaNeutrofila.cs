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
    public GameObject efeitoExplosaoPrefab;

    private Transform alvoAtual;
    private bool estaExplodindo = false;

    void Update()
    {
        if (estaExplodindo) return;

        BuscarAlvoMaisProximo();

        if (alvoAtual != null)
        {
            AtualizarAnimacao(true);

            transform.position = Vector2.MoveTowards(
                transform.position, 
                alvoAtual.position, 
                velocidade * Time.deltaTime
            );

            Vector2 direcao = (alvoAtual.position - transform.position).normalized;
            float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angulo);

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

        if (animadorTropa != null)
        {
            animadorTropa.SetTrigger("Explodir");
        }

        if (efeitoExplosaoPrefab != null)
        {
            Instantiate(efeitoExplosaoPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] acertos = Physics2D.OverlapCircleAll(transform.position, raioExplosao);
        foreach (Collider2D col in acertos)
        {
            Inimigo inimigo = col.GetComponent<Inimigo>();
            if (inimigo != null)
            {
                inimigo.SendMessage("ReceberDano", dano, SendMessageOptions.DontRequireReceiver);
            }
        }

        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}