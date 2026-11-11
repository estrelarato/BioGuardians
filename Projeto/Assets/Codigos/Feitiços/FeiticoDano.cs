using UnityEngine;

public class FeiticoDano : FeiticoBase
{
    [Header("Parâmetros de Dano / Área")]
    public float danoDoFeitico = 50f;
    public float raioDeAlcance = 2.5f;
    public TipoDano tipoDeDano = TipoDano.Verdadeiro;
    public LayerMask mascaraInimigos;
    public GameObject prefabEfeitoArea;

    protected override void ExecutarLogicaInicial()
    {
        // Ativa o modo de mira no mapa
        mirandoNaTela = true;
        Debug.Log($"[FEITIÇO DANO] {nomeFeitico} selecionado! Clique no mapa para lançar.");
    }

    protected override void AplicarEfeitoNoPonto(Vector3 posicaoMundo)
    {
        if (prefabEfeitoArea != null)
        {
            Instantiate(prefabEfeitoArea, posicaoMundo, Quaternion.identity);
        }

        // Procura por todos os colisores na área selecionada
        Collider2D[] alvos = Physics2D.OverlapCircleAll(posicaoMundo, raioDeAlcance, mascaraInimigos);

        Debug.Log($"[FEITIÇO] Colisores encontrados no raio: {alvos.Length}");

        foreach (Collider2D alvo in alvos)
        {
            Debug.Log($"[FEITIÇO] Acertou o objeto: {alvo.name}");

            // Procura o script Inimigo no próprio objeto ou nos seus pais (útil para prefabs com estruturas filho)
            Inimigo inimigo = alvo.GetComponentInParent<Inimigo>();
            if (inimigo != null)
            {
                Debug.Log($"[FEITIÇO] Script Inimigo encontrado em {alvo.name}! Aplicando dano...");
                inimigo.ReceberDano(danoDoFeitico, tipoDeDano);
            }
            else
            {
                Debug.LogWarning($"[FEITIÇO] Colisor {alvo.name} detectado, mas NENHUM script 'Inimigo' foi achado nele ou nos pais!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDeAlcance);
    }
}