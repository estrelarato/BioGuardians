using System.Collections;
using UnityEngine;

public class EfeitoLento : MonoBehaviour
{
    private Inimigo inimigo;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;

    private float velocidadeBase;
    private float porcentagemLentoAtual = 0f;
    private float tempoRestante = 0f;
    private Coroutine corrotinaLento;

    // Verde chamativo para indicar a lentidão
    private Color corVerdeLento = new Color(0.2f, 1f, 0.2f, 1f);

    private void Awake()
    {
        inimigo = GetComponent<Inimigo>();
        if (inimigo != null)
        {
            velocidadeBase = inimigo.velocidade;
        }

        // Pega o SpriteRenderer mesmo se estiver num objeto filho (ex: sprite da bactéria)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("[EFEITO LENTO] Nenhum SpriteRenderer foi encontrado no Inimigo ou nos seus filhos!");
        }
        else
        {
            corOriginal = spriteRenderer.color;
        }
    }

    /// <summary>
    /// Aplica ou renova a lentidão sem multiplicar a redução de velocidade
    /// </summary>
    public void AplicarLentidao(float porcentagemLento, float duracao)
    {
        if (inimigo == null) return;

        // Se a nova lentidão for maior, atualiza a velocidade com base na velocidade base
        if (porcentagemLento > porcentagemLentoAtual)
        {
            porcentagemLentoAtual = porcentagemLento;
            inimigo.velocidade = velocidadeBase * (1f - porcentagemLentoAtual);
        }

        // Aplica a cor verde no SpriteRenderer
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corVerdeLento;
        }

        // Renova o tempo de duração da lentidão
        tempoRestante = duracao;

        // Se já houver um timer rodando, interrompe para reiniciar a contagem com o novo tempo
        if (corrotinaLento != null)
        {
            StopCoroutine(corrotinaLento);
        }

        corrotinaLento = StartCoroutine(ProcessarLentidao());
    }

    private IEnumerator ProcessarLentidao()
    {
        while (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
            yield return null;
        }

        RestaurarEstadoOriginal();
    }

    private void RestaurarEstadoOriginal()
    {
        // Restaura a velocidade original do inimigo
        if (inimigo != null)
        {
            inimigo.velocidade = velocidadeBase;
        }

        // Restaura a cor original do sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginal;
        }

        Destroy(this);
    }

    private void OnDestroy()
    {
        // Garantia de segurança ao destruir o componente ou o inimigo
        if (inimigo != null)
        {
            inimigo.velocidade = velocidadeBase;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginal;
        }
    }
}