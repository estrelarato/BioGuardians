using UnityEngine;

public class Projetil : MonoBehaviour
{
    [Header("Atributos do Projétil")]
    public float velocidade = 12f;
    public float dano = 15f;

    private Inimigo alvo;

    /// <summary>
    /// Chamado pela Torre para passar qual inimigo deve ser atingido.
    /// </summary>
    public void DefinirAlvo(Inimigo novoAlvo)
    {
        alvo = novoAlvo;
    }

    void Update()
    {
        // Se o inimigo morrer antes do impacto, destrói a bala
        if (alvo == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move a bala na direção do inimigo
        transform.position = Vector2.MoveTowards(
            transform.position, 
            alvo.transform.position, 
            velocidade * Time.deltaTime
        );

        // Checa a distância para registrar o impacto
        float distancia = Vector2.Distance(transform.position, alvo.transform.position);

        if (distancia <= 0.2f)
        {
            AtingirAlvo();
        }
    }

    void AtingirAlvo()
    {
        if (alvo != null)
        {
            // AVISO NO CONSOLE: Mostra se a bala realmente encontrou o script do Inimigo e quanto de dano tentou aplicar
            Debug.Log($"[PROJÉTIL] Atingiu o inimigo {alvo.name}! Tentando aplicar {dano} de dano.");

            // Aplica o dano no script do Inimigo
            alvo.ReceberDano(dano);
        }
        else
        {
            Debug.LogWarning("[PROJÉTIL] Impactou, mas a referência do Alvo estava NULA!");
        }

        // Destrói a bala após o acerto
        Destroy(gameObject);
    }
}