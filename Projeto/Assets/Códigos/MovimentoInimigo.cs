using UnityEngine;

public class MovimentoInimigo : MonoBehaviour
{
    private Transform[] pontosDeCaminho;
    private int indiceAtual = 0;

    private Inimigo inimigo;

    void Start()
    {
        inimigo = GetComponent<Inimigo>();
    }

    public void DefinirCaminho(Transform[] novosPontos)
    {
        pontosDeCaminho = novosPontos;
    }

    void Update()
    {
        if (pontosDeCaminho == null || pontosDeCaminho.Length == 0)
            return;

        Mover();

        if (pontosDeCaminho == null)
        {
        Debug.Log("SEM CAMINHO!");
        }
    }

    void Mover()
    {
        if (indiceAtual >= pontosDeCaminho.Length)
        {
            ChegouNoFinal();
            return;
        }

        Transform alvo = pontosDeCaminho[indiceAtual];

        transform.position = Vector2.MoveTowards(
            transform.position,
            alvo.position,
            inimigo.velocidade * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, alvo.position) < 0.1f)
        {
            indiceAtual++;
        }
    }

    void ChegouNoFinal()
    {
        Destroy(gameObject);
    }
}