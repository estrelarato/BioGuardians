using UnityEngine;

public class Projetil : MonoBehaviour
{
    [Header("Atributos do Projétil")]
    public float velocidade = 12f;
    public float dano = 15f;
    public TipoDano tipoDano = TipoDano.Fagocito;

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

        transform.position = Vector2.MoveTowards(
            transform.position, 
            alvo.transform.position, 
            velocidade * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, alvo.transform.position) <= 0.2f)
        {
            AtingirAlvo();
        }
    }

    void AtingirAlvo()
    {
        if (alvo != null)
        {
            alvo.ReceberDano(dano, tipoDano);
        }

        Destroy(gameObject);
    }
}