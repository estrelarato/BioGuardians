using System.Collections;
using UnityEngine;

public class EfeitoLento : MonoBehaviour
{
    private Coroutine corrotinaLento;

    /// <summary>
    /// Aplica a lentidão ao inimigo. Se já estiver desacelerado, renova o tempo.
    /// </summary>
    public void AplicarLentidao(float porcentagemLento, float duracao)
    {
        if (corrotinaLento != null)
        {
            StopCoroutine(corrotinaLento);
        }

        corrotinaLento = StartCoroutine(ProcessarLentidao(porcentagemLento, duracao));
    }

    private IEnumerator ProcessarLentidao(float porcentagemLento, float duracao)
    {
        Inimigo inimigo = GetComponent<Inimigo>();

        if (inimigo != null)
        {
            // Guarda a velocidade original antes de aplicar o efeito
            float velocidadeOriginal = inimigo.velocidade;
            
            // Reduz a velocidade (ex: 0.4 de porcentagem reduz para 60% da velocidade)
            inimigo.velocidade = velocidadeOriginal * (1f - porcentagemLento);

            yield return new WaitForSeconds(duracao);

            // Restaura a velocidade normal ao fim da duração
            inimigo.velocidade = velocidadeOriginal;
        }

        Destroy(this); // Remove o componente de efeito do inimigo
    }
}