using UnityEngine;

public class InimigoVoador : Inimigo
{
    [Header("Mecânica Aérea")]
    public bool ehVoador = true;
    public Transform alvoFinalPulmao; // O ponto final direto para onde ele voa
    
    [Tooltip("Dano causado à base ao chegar ao fim")]
    public int danoNaBase = 1;

    private bool chegouAoFim = false;

    private void Update()
    {
        if (chegouAoFim) return;

        // Se for voador e tiver um alvo final, vai direto a ele ignorando os waypoints
        if (ehVoador && alvoFinalPulmao != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, alvoFinalPulmao.position, velocidade * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, alvoFinalPulmao.position) < 0.1f)
            {
                AtingiuBase();
            }
        }
    }

    private void AtingiuBase()
    {
        chegouAoFim = true;

        VidaDoJogador sistemaVida = FindFirstObjectByType<VidaDoJogador>();
        if (sistemaVida != null)
        {
            sistemaVida.TomarDano(danoNaBase);
        }

        Destroy(gameObject);
    }
}