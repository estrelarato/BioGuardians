using UnityEngine;

public class ProjetilExplosivo : MonoBehaviour
{
    public float velocidade = 10f;
    public float dano = 25f;
    
    [Header("Configurações da Explosão")]
    public float raioExplosao = 1.5f;
    public LayerMask layerInimigos;

    [Header("Efeitos Visuais")]
    public GameObject prefabEfeitoExplosao; 

    private Inimigo alvo;

    public void DefinirAlvo(Inimigo novoAlvo)
    {
        alvo = novoAlvo;
    }

    void Update()
    {
        if (alvo == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direcao = (alvo.transform.position - transform.position).normalized;
        transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);

        float distancia = Vector2.Distance(transform.position, alvo.transform.position);

        if (distancia < 0.2f)
        {
            Explodir();
        }
    }

    void Explodir()
    {
        // Cria o efeito visual na posição do impacto
        if (prefabEfeitoExplosao != null)
        {
            GameObject efeitoObj = Instantiate(prefabEfeitoExplosao, transform.position, Quaternion.identity);
            
            // CORREÇÃO: Força o sistema de partículas a reiniciar e rodar em TODOS os tiros
            ParticleSystem sistemaParticulas = efeitoObj.GetComponent<ParticleSystem>();
            if (sistemaParticulas != null)
            {
                sistemaParticulas.Clear(); 
                sistemaParticulas.Play();  
            }
        }

        // Sistema de dano em área por círculo de colisão
        Collider2D[] objetosAtingidos = Physics2D.OverlapCircleAll(transform.position, raioExplosao, layerInimigos);

        foreach (Collider2D colisor in objetosAtingidos)
        {
            Inimigo inimigo = colisor.GetComponent<Inimigo>();

            if (inimigo != null)
            {
                inimigo.ReceberDano(dano);
            }
        }
        
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}