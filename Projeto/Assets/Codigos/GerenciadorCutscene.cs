using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorCutsceneQuadrinhos : MonoBehaviour
{
    [System.Serializable]
    public class PaginaQuadrinhos
    {
        [Tooltip("Arraste as Images (UI) dos quadrinhos desta página na ordem em que devem aparecer")]
        public List<CanvasGroup> quadrosDaPagina = new List<CanvasGroup>();
    }

    [Header("Páginas e Quadrinhos")]
    public List<PaginaQuadrinhos> paginas = new List<PaginaQuadrinhos>();
    public string nomeProximaCena = "Fase1";

    [Header("Animação de Entrada do Quadrinho")]
    public float duracaoAparicao = 0.3f;
    public float escalaInicialQuadro = 0.92f;
    [Tooltip("Se marcado, avança sozinho após alguns segundos além de aceitar clique")]
    public bool avancoAutomatico = false;
    public float tempoPorQuadroAuto = 2.5f;

    [Header("Controles")]
    public Button botaoAvancarTelaInteira;
    public Button botaoPular;

    private int paginaAtual = 0;
    private int quadroAtualNaPagina = -1;
    private bool animando = false;
    private float timerAuto = 0f;

    void Start()
    {
        if (botaoAvancarTelaInteira != null)
            botaoAvancarTelaInteira.onClick.AddListener(Avancar);

        if (botaoPular != null)
            botaoPular.onClick.AddListener(CarregarProximaCena);

        EsconderTodosOsQuadros();

        if (paginas.Count > 0 && paginas[0].quadrosDaPagina.Count > 0)
        {
            Avancar();
        }
        else
        {
            CarregarProximaCena();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Avancar();
        }

        if (avancoAutomatico && !animando)
        {
            timerAuto += Time.deltaTime;
            if (timerAuto >= tempoPorQuadroAuto)
            {
                Avancar();
            }
        }
    }

    private void EsconderTodosOsQuadros()
    {
        foreach (var pagina in paginas)
        {
            foreach (var quadro in pagina.quadrosDaPagina)
            {
                if (quadro != null)
                {
                    quadro.alpha = 0f;
                    quadro.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Avancar()
    {
        if (animando) return;
        timerAuto = 0f;

        quadroAtualNaPagina++;

        // Se terminou os quadrinhos da página atual, vai para a próxima página
        if (quadroAtualNaPagina >= paginas[paginaAtual].quadrosDaPagina.Count)
        {
            // Esconde os quadrinhos da página anterior
            foreach (var q in paginas[paginaAtual].quadrosDaPagina)
            {
                if (q != null) q.gameObject.SetActive(false);
            }

            paginaAtual++;
            quadroAtualNaPagina = 0;

            // Se acabou todas as páginas, inicia a Fase!
            if (paginaAtual >= paginas.Count)
            {
                CarregarProximaCena();
                return;
            }
        }

        CanvasGroup proximoQuadro = paginas[paginaAtual].quadrosDaPagina[quadroAtualNaPagina];
        StartCoroutine(RevelarQuadro(proximoQuadro));
    }

    private IEnumerator RevelarQuadro(CanvasGroup quadro)
    {
        if (quadro == null) yield break;

        animando = true;
        quadro.gameObject.SetActive(true);
        quadro.alpha = 0f;

        RectTransform rect = quadro.GetComponent<RectTransform>();
        Vector3 escalaAlvo = Vector3.one;
        Vector3 escalaInicio = Vector3.one * escalaInicialQuadro;

        float tempo = 0f;
        while (tempo < duracaoAparicao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tempo / duracaoAparicao);

            quadro.alpha = t;
            if (rect != null)
                rect.localScale = Vector3.Lerp(escalaInicio, escalaAlvo, t);

            yield return null;
        }

        quadro.alpha = 1f;
        if (rect != null) rect.localScale = escalaAlvo;
        animando = false;
    }

    public void CarregarProximaCena()
    {
        SceneManager.LoadScene(nomeProximaCena);
    }
}