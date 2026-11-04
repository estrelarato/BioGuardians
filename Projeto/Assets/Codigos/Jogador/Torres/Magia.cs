using UnityEngine;

public class Magia : MonoBehaviour
{
    [Header("Atributos do Projétil")]
    public float velocidade = 12f;
    public float dano = 15f;

    [Tooltip("Escolha o tipo de dano deste projétil no Inspector")]
    public TipoDano tipoDano = TipoDano.Fagocito; 

    private Inimigo alvo;
    private bool estaPronto = false;

    public void DefinirAlvo(Inimigo novoAlvo)
    {
        alvo = novoAlvo;
    }

    public void LiberarTiro()
    {
        estaPronto = true;
    }

    void Update()
    {
        // Não se move enquanto estiver no processo de carga
        if (!estaPronto) return;

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