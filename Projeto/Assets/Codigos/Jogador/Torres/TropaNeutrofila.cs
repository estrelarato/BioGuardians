using System.Collections;
using UnityEngine;

public class TropaNeutrofila : MonoBehaviour
{
    [Header("Configurações do Minion")]
    public float velocidade = 4f;
    public float dano = 25f;
    public float raioExplosao = 1.5f;
    public float raioDeteccao = 5f;
    public float distanciaParaExplodir = 0.5f;

    [Header("Tipo de Dano (Biológico)")]
    [Tooltip("Escolha o tipo de dano provocado pela explosão/desgranulação")]
    public TipoDano tipoDano = TipoDano.Enzimatico;

    [Header("Componentes e Efeitos")]
    public Animator animadorTropa;
    public SpriteRenderer spriteRenderer;
    public GameObject efeitoExplosaoPrefab;

    [Header("Efeito Visual do Raio de Explosão")]
    public Color corFlashAzul = new Color(0.15f, 0.75f, 1f, 0.7f);
    public Color corFlashBranco = new Color(1f, 1f, 1f, 0.85f);
    public float duracaoFlashExplosao = 0.35f;
    public float velocidadePiscada = 0.045f;
    public int ordemCamadaEfeito = 15;

    private Transform alvoAtual;
    private bool estaExplodindo = false;
    private bool impactoJaExecutado = false;
    private static Sprite spriteCirculoCache;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (estaExplodindo) return;

        BuscarAlvoMaisProximo();

        if (alvoAtual != null)
        {
            AtualizarAnimacao(true);

            // Move-se diretamente para o alvo
            transform.position = Vector2.MoveTowards(
                transform.position, 
                alvoAtual.position, 
                velocidade * Time.deltaTime
            );

            // Flip do sprite baseado no movimento horizontal
            float direcaoX = alvoAtual.position.x - transform.position.x;
            AtualizarFlipSprite(direcaoX);

            float distancia = Vector2.Distance(transform.position, alvoAtual.position);
            if (distancia <= distanciaParaExplodir)
            {
                Explodir();
            }
        }
        else
        {
            AtualizarAnimacao(false);
        }
    }

    void AtualizarFlipSprite(float direcaoX)
    {
        if (Mathf.Abs(direcaoX) > 0.05f && spriteRenderer != null)
        {
            spriteRenderer.flipX = (direcaoX < 0);
        }
    }

    void AtualizarAnimacao(bool estaMovendo)
    {
        if (animadorTropa != null)
        {
            animadorTropa.SetBool("EstaAndando", estaMovendo);
        }
    }

    void BuscarAlvoMaisProximo()
    {
        Inimigo[] inimigos = FindObjectsOfType<Inimigo>();
        float menorDistancia = Mathf.Infinity;
        Transform melhorAlvo = null;

        foreach (Inimigo inimigo in inimigos)
        {
            float dist = Vector2.Distance(transform.position, inimigo.transform.position);
            if (dist < menorDistancia && dist <= raioDeteccao)
            {
                menorDistancia = dist;
                melhorAlvo = inimigo.transform;
            }
        }

        alvoAtual = melhorAlvo;
    }

    // 1. Inicia apenas a animação de explosão e para o movimento
    void Explodir()
    {
        estaExplodindo = true;
        AtualizarAnimacao(false);

        if (animadorTropa != null)
        {
            animadorTropa.SetTrigger("Explodir");
        }
        else
        {
            // Fallback caso não haja Animator configurado
            ExecutarImpactoExplosao();
        }
    }

    // 2. Chame este método via Animation Event no frame exato em que a explosão acontece
    public void ExecutarImpactoExplosao()
    {
        if (impactoJaExecutado) return;
        impactoJaExecutado = true;

        // Desativa colisor para não causar múltiplos impactos
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (efeitoExplosaoPrefab != null)
        {
            Instantiate(efeitoExplosaoPrefab, transform.position, Quaternion.identity);
        }

        // Cria o flash circular com o tamanho exato de raioExplosao
        CriarEfeitoRaioExplosao();

        Collider2D[] acertos = Physics2D.OverlapCircleAll(transform.position, raioExplosao);
        foreach (Collider2D colisor in acertos)
        {
            Inimigo inimigo = colisor.GetComponent<Inimigo>();
            if (inimigo != null)
            {
                inimigo.ReceberDano(dano, tipoDano);
            }
        }
    }

    private void CriarEfeitoRaioExplosao()
    {
        GameObject objFlash = new GameObject("FlashRaioExplosao");
        objFlash.transform.position = transform.position;
        float diametro = raioExplosao * 2f;
        objFlash.transform.localScale = new Vector3(diametro, diametro, 1f);

        SpriteRenderer sr = objFlash.AddComponent<SpriteRenderer>();
        sr.sprite = ObterSpriteCirculo();
        sr.sortingOrder = ordemCamadaEfeito;
        if (spriteRenderer != null)
        {
            sr.sortingLayerID = spriteRenderer.sortingLayerID;
        }

        FlashExplosaoController controlador = objFlash.AddComponent<FlashExplosaoController>();
        controlador.Iniciar(sr, corFlashBranco, corFlashAzul, duracaoFlashExplosao, velocidadePiscada);
    }

    private static Sprite ObterSpriteCirculo()
    {
        if (spriteCirculoCache != null) return spriteCirculoCache;

        int tamanho = 128;
        Texture2D tex = new Texture2D(tamanho, tamanho, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        float centro = (tamanho - 1) * 0.5f;
        float raio = tamanho * 0.5f;

        for (int y = 0; y < tamanho; y++)
        {
            for (int x = 0; x < tamanho; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
                if (dist <= raio)
                {
                    float bordaSuave = Mathf.Clamp01((raio - dist) / 2f);
                    float gradienteBorda = Mathf.Lerp(0.45f, 1f, Mathf.Pow(dist / raio, 1.5f));
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, bordaSuave * gradienteBorda));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();
        spriteCirculoCache = Sprite.Create(tex, new Rect(0, 0, tamanho, tamanho), new Vector2(0.5f, 0.5f), tamanho);
        return spriteCirculoCache;
    }

    // 3. Chame este método via Animation Event no último frame de 'N-explode'
    public void DestruirAposAnimacao()
    {
        // Garante que o dano/efeito ocorreu mesmo se o evento de impacto tiver sido pulado
        if (!impactoJaExecutado)
        {
            ExecutarImpactoExplosao();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}

public class FlashExplosaoController : MonoBehaviour
{
    public void Iniciar(SpriteRenderer sr, Color branco, Color azul, float duracaoTotal, float intervaloPiscada)
    {
        StartCoroutine(AnimarFlash(sr, branco, azul, duracaoTotal, intervaloPiscada));
    }

    private IEnumerator AnimarFlash(SpriteRenderer sr, Color branco, Color azul, float duracaoTotal, float intervaloPiscada)
    {
        float tempoDecorrido = 0f;
        float timerPiscada = 0f;
        bool mostrarBranco = true;
        sr.color = branco;

        while (tempoDecorrido < duracaoTotal)
        {
            float dt = Time.deltaTime;
            tempoDecorrido += dt;
            timerPiscada += dt;

            if (timerPiscada >= intervaloPiscada)
            {
                timerPiscada = 0f;
                mostrarBranco = !mostrarBranco;
            }

            float progresso = tempoDecorrido / duracaoTotal;
            Color corBase = mostrarBranco ? branco : azul;
            float alphaAtual = Mathf.Lerp(corBase.a, 0f, progresso * progresso);

            sr.color = new Color(corBase.r, corBase.g, corBase.b, alphaAtual);
            yield return null;
        }

        Destroy(gameObject);
    }
}