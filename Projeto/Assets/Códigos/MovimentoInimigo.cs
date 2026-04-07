using UnityEngine;

public class MovimentoInimigo : MonoBehaviour
{
    public Transform[] pontosDeCaminho;
    private int indiceAtual = 0;

    private Inimigo inimigo;

    void Start()
    {
        inimigo = GetComponent<Inimigo>();
    }

    void Update()
    {
        Mover();
    }

    void Mover()
    {
        if (indiceAtual >= pontosDeCaminho.Length)
        {
            ChegouNoFinal();
            return;
        }

        Transform alvo = pontosDeCaminho[indiceAtual];

        transform.position = Vector3.MoveTowards(
            transform.position,
            alvo.position,
            inimigo.velocidade * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, alvo.position) < 0.1f)
        {
            indiceAtual++;
        }
    }

    void ChegouNoFinal()
    {
        // Exemplo:
        // GerenciadorJogo.Instancia.ReceberDano(inimigo.dano);

        Destroy(gameObject);
    }
}