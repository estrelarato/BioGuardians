using System.Collections;
using UnityEngine;

public class EfeitoFeiticoArea : MonoBehaviour
{
    [Header("Tipo de Efeito")]
    public bool ehDanoInstantaneo = true;
    public bool ehAreaContinuada = false; // Corrigido: removidos os parênteses do nome

    [Header("Atributos de Dano")]
    public float danoOuDanoPorSegundo = 80f;
    public TipoDano tipoDano = TipoDano.Enzimatico;
    public float raioDeAlcance = 2.5f;
    public LayerMask layerInimigos;

    [Header("Duração (Apenas para Área Continuada)")]
    public float duracaoDoEfeito = 5f;
    public float intervaloTickDano = 0.5f; // Aplica dano a cada 0.5s

    [Header("Efeito Visual")]
    public GameObject efeitoVisualPrefab;

    void Start()
    {
        if (efeitoVisualPrefab != null)
        {
            Instantiate(efeitoVisualPrefab, transform.position, Quaternion.identity);
        }

        if (ehDanoInstantaneo)
        {
            AplicarDanoInstantaneo();
            Destroy(gameObject, 1f);
        }
        else if (ehAreaContinuada)
        {
            StartCoroutine(RotinaDanoContinuo());
        }
    }

    void AplicarDanoInstantaneo()
    {
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(transform.position, raioDeAlcance, layerInimigos);
        foreach (Collider2D col in atingidos)
        {
            Inimigo inimigo = col.GetComponent<Inimigo>();
            if (inimigo != null)
            {
                inimigo.ReceberDano(danoOuDanoPorSegundo, tipoDano);
            }
        }
    }

    IEnumerator RotinaDanoContinuo()
    {
        float tempoDecorrito = 0f;

        while (tempoDecorrito < duracaoDoEfeito)
        {
            Collider2D[] atingidos = Physics2D.OverlapCircleAll(transform.position, raioDeAlcance, layerInimigos);
            foreach (Collider2D col in atingidos)
            {
                Inimigo inimigo = col.GetComponent<Inimigo>();
                if (inimigo != null)
                {
                    float danoDoTick = danoOuDanoPorSegundo * intervaloTickDano;
                    inimigo.ReceberDano(danoDoTick, tipoDano);
                }
            }

            yield return new WaitForSeconds(intervaloTickDano);
            tempoDecorrito += intervaloTickDano;
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, raioDeAlcance);
    }
}