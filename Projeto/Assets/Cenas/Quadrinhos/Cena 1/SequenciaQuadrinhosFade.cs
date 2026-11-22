using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SequenciaQuadrinhosPaginada : MonoBehaviour
{
    [Header("Todos os Quadrinhos na Ordem (1, 2, 3... 7, 8...)")]
    [Tooltip("Arraste todos os objetos dos quadrinhos em sequência. A cada 6 (ou o tamanho da página), a tela limpa para o próximo grupo.")]
    public List<GameObject> todosQuadrinhos = new List<GameObject>();

    [Tooltip("Quantos quadrinhos aparecem por página antes de apagar para o próximo grupo")]
    public int quadrosPorPagina = 6;

    [Header("Tempos da Cutscene")]
    [Tooltip("Segundos de espera antes do primeiro quadrinho surgir")]
    public float atrasoInicial = 0.5f;
    [Tooltip("Tempo (em segundos) para cada quadrinho surgir (efeito fantasma)")]
    public float duracaoFadeIn = 1.0f;
    [Tooltip("Tempo (em segundos) de pausa entre um quadrinho e o próximo")]
    public float intervaloEntreQuadros = 1.5f;
    [Tooltip("Tempo de pausa após o 6º quadrinho da página antes de apagar tudo para a próxima página")]
    public float pausaAntesDeLimparPagina = 2.0f;
    [Tooltip("Tempo (em segundos) para os 6 quadrinhos apagarem juntos")]
    public float duracaoFadeOutPagina = 0.6f;

    [Header("Próxima Cena")]
    public string nomeProximaFase = "Fase1";
    [Tooltip("Permite clicar ou apertar Espaço para pular a cutscene")]
    public bool permitirPularComClique = true;

    private bool pulando = false;

    void Start()
    {
        PrepararTodosInvisiveis();
        StartCoroutine(ReproduzirSequenciaPaginada());
    }

    void Update()
    {
        if (permitirPularComClique && !pulando)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                pulando = true;
                SceneManager.LoadScene(nomeProximaFase);
            }
        }
    }

    private void PrepararTodosInvisiveis()
    {
        foreach (GameObject obj in todosQuadrinhos)
        {
            if (obj == null) continue;
            DefinirAlpha(obj, 0f);
            obj.SetActive(false);
        }
    }

    private IEnumerator ReproduzirSequenciaPaginada()
    {
        if (atrasoInicial > 0f)
            yield return new WaitForSeconds(atrasoInicial);

        int total = todosQuadrinhos.Count;

        // Percorre de página em página (0..5, depois 6..11, depois 12..17...)
        for (int inicioPagina = 0; inicioPagina < total; inicioPagina += quadrosPorPagina)
        {
            int fimPagina = Mathf.Min(inicioPagina + quadrosPorPagina, total);
            List<GameObject> grupoAtual = new List<GameObject>();

            // 1. Ativa os objetos desta página (ainda invisíveis)
            for (int i = inicioPagina; i < fimPagina; i++)
            {
                GameObject q = todosQuadrinhos[i];
                if (q != null)
                {
                    DefinirAlpha(q, 0f);
                    q.SetActive(true);
                    grupoAtual.Add(q);
                }
            }

            // 2. Faz cada quadrinho da página atual aparecer um por um
            for (int i = 0; i < grupoAtual.Count; i++)
            {
                yield return StartCoroutine(FadeQuadro(grupoAtual[i], 0f, 1f, duracaoFadeIn));

                if (i < grupoAtual.Count - 1 && intervaloEntreQuadros > 0f)
                {
                    yield return new WaitForSeconds(intervaloEntreQuadros);
                }
            }

            // 3. Pausa para o jogador ler/ver a página completa
            if (pausaAntesDeLimparPagina > 0f)
            {
                yield return new WaitForSeconds(pausaAntesDeLimparPagina);
            }

            bool temProximaPagina = (fimPagina < total);

            // 4. Se ainda há mais páginas (7..12, 13..18...), apaga os 6 atuais juntos
            if (temProximaPagina)
            {
                yield return StartCoroutine(FadeGrupoSimultaneo(grupoAtual, 1f, 0f, duracaoFadeOutPagina));

                foreach (GameObject q in grupoAtual)
                {
                    if (q != null) q.SetActive(false);
                }
            }
        }

        if (!pulando)
        {
            SceneManager.LoadScene(nomeProximaFase);
        }
    }

    private IEnumerator FadeQuadro(GameObject obj, float alphaInicial, float alphaFinal, float duracao)
    {
        if (obj == null) yield break;

        float tempo = 0f;
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float alpha = Mathf.SmoothStep(alphaInicial, alphaFinal, tempo / duracao);
            DefinirAlpha(obj, alpha);
            yield return null;
        }

        DefinirAlpha(obj, alphaFinal);
    }

    private IEnumerator FadeGrupoSimultaneo(List<GameObject> grupo, float alphaInicial, float alphaFinal, float duracao)
    {
        float tempo = 0f;
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float alpha = Mathf.SmoothStep(alphaInicial, alphaFinal, tempo / duracao);

            for (int i = 0; i < grupo.Count; i++)
            {
                if (grupo[i] != null) DefinirAlpha(grupo[i], alpha);
            }

            yield return null;
        }

        for (int i = 0; i < grupo.Count; i++)
        {
            if (grupo[i] != null) DefinirAlpha(grupo[i], alphaFinal);
        }
    }

    private void DefinirAlpha(GameObject obj, float alpha)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = alpha;
            return;
        }

        Graphic uiGraphic = obj.GetComponent<Graphic>();
        if (uiGraphic != null)
        {
            Color c = uiGraphic.color;
            c.a = alpha;
            uiGraphic.color = c;
            return;
        }

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}