using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 10f;
    public float dano = 10f;

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
            AtingirAlvo();
        }
    }

    void AtingirAlvo()
    {
        if (alvo != null)
        {
            alvo.ReceberDano(dano);
        }

        Destroy(gameObject);
    }
}