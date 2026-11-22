using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Inimigo))]
public class BossSARM : MonoBehaviour
{
    [Header("Configurações de Áudio e Visual")]
    public AudioClip musicaBoss;
    public SpriteRenderer spriteRenderer;
    public Color corNormal = Color.white;
    public Color corBiofilme = new Color(1f, 0.85f, 0.2f, 1f);

    [Header("Passiva: Escudo de Biofilme")]
    public float intervaloBiofilme = 15f;
    public float duracaoBiofilme = 5f;
    public float resistenciaComBiofilme = 0.1f;

    [Header("Ativa 1: Leucocidina (Nuvem Tóxica Procedural)")]
    public float intervaloToxina = 8f;
    public float raioToxina = 3f;
    public float duracaoStunTropas = 2f;
    public Color corNuvemToxica = new Color(0.65f, 0.15f, 0.85f, 0.65f);
    public float duracaoVisualNuvem = 1.2f;
    public int ordemCamadaNuvem = 12;

    [Header("Ativa 2: Invocação de Inimigos (Spawn Periódico)")]
    [Tooltip("Arraste o prefab do StAu ou outros inimigos que ele vai invocar")]
    public GameObject[] prefabsInimigosSpawn;
    public int quantidadePorSpawn = 3;
    public float intervaloSpawnInimigos = 10f;
    public float raioEspalhamento = 0.6f;

    private Inimigo inimigo;
    private float resFagocitoOriginal;
    private float resEnzimaticoOriginal;

    private float timerBiofilme;
    private float timerToxina;
    private float timerSpawn;
    private bool biofilmeAtivo = false;

    private static Sprite spriteNuvemCache;

    void Awake()
    {
        inimigo = GetComponent<Inimigo>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        resFagocitoOriginal = inimigo.resistenciaFagocito;
        resEnzimaticoOriginal = inimigo.resistenciaEnzimatico;

        timerBiofilme = intervaloBiofilme * 0.5f;
        timerToxina = intervaloToxina;
        timerSpawn = intervaloSpawnInimigos * 0.5f; // Faz o primeiro spawn mais cedo

        if (AudioManager.Instancia != null && musicaBoss != null)
        {
            AudioManager.Instancia.TocarMusica(musicaBoss);
        }
    }

    void Update()
    {
        if (inimigo.EstaMorto) return;

        // 1. Spawn periódico de inimigos
        timerSpawn -= Time.deltaTime;
        if (timerSpawn <= 0f)
        {
            SpawnarInimigos();
            timerSpawn = intervaloSpawnInimigos;
        }

        // 2. Escudo de Biofilme
        if (!biofilmeAtivo)
        {
            timerBiofilme -= Time.deltaTime;
            if (timerBiofilme <= 0f)
            {
                StartCoroutine(AtivarBiofilme());
            }
        }

        // 3. Toxina Leucocidina
        timerToxina -= Time.deltaTime;
        if (timerToxina <= 0f)
        {
            LiberarLeucocidina();
            timerToxina = intervaloToxina;
        }
    }

    private IEnumerator AtivarBiofilme()
    {
        biofilmeAtivo = true;

        inimigo.resistenciaFagocito = resistenciaComBiofilme;
        inimigo.resistenciaEnzimatico = resistenciaComBiofilme;

        EfeitoLento lento = GetComponent<EfeitoLento>();
        if (lento != null) Destroy(lento);

        if (spriteRenderer != null) spriteRenderer.color = corBiofilme;

        yield return new WaitForSeconds(duracaoBiofilme);

        inimigo.resistenciaFagocito = resFagocitoOriginal;
        inimigo.resistenciaEnzimatico = resEnzimaticoOriginal;

        if (spriteRenderer != null) spriteRenderer.color = corNormal;

        biofilmeAtivo = false;
        timerBiofilme = intervaloBiofilme;
    }

    private void LiberarLeucocidina()
    {
        CriarVisualNuvemToxica();

        Collider2D[] atingidos = Physics2D.OverlapCircleAll(transform.position, raioToxina);
        foreach (Collider2D col in atingidos)
        {
            TropaNeutrofila neutrofila = col.GetComponentInParent<TropaNeutrofila>();
            if (neutrofila != null)
            {
                Destroy(neutrofila.gameObject);
                continue;
            }

            TropaBasofila basofila = col.GetComponentInParent<TropaBasofila>();
            if (basofila != null)
            {
                StartCoroutine(AtordoarComponente(basofila, duracaoStunTropas));
            }

            TropaEosinofila eosinofila = col.GetComponentInParent<TropaEosinofila>();
            if (eosinofila != null)
            {
                StartCoroutine(AtordoarComponente(eosinofila, duracaoStunTropas));
            }
        }
    }

    private void SpawnarInimigos()
    {
        if (prefabsInimigosSpawn == null || prefabsInimigosSpawn.Length == 0) return;

        for (int i = 0; i < quantidadePorSpawn; i++)
        {
            GameObject prefabEscolhido = prefabsInimigosSpawn[Random.Range(0, prefabsInimigosSpawn.Length)];
            if (prefabEscolhido == null) continue;

            Vector3 offset = (Vector3)(Random.insideUnitCircle * raioEspalhamento);
            GameObject novoInimigo = Instantiate(prefabEscolhido, transform.position + offset, Quaternion.identity);

            Inimigo scriptInimigo = novoInimigo.GetComponent<Inimigo>();
            if (scriptInimigo != null)
            {
                scriptInimigo.DefinirCaminho(inimigo.pontosCaminho, inimigo.IndicePontoAtual);
            }
        }
    }

    private void CriarVisualNuvemToxica()
    {
        GameObject objNuvem = new GameObject("NuvemLeucocidina");
        objNuvem.transform.position = transform.position;

        SpriteRenderer sr = objNuvem.AddComponent<SpriteRenderer>();
        sr.sprite = ObterSpriteNuvem();
        sr.color = corNuvemToxica;
        sr.sortingOrder = ordemCamadaNuvem;

        if (spriteRenderer != null)
        {
            sr.sortingLayerID = spriteRenderer.sortingLayerID;
        }

        StartCoroutine(AnimarNuvemToxica(objNuvem, sr));
    }

    private IEnumerator AnimarNuvemToxica(GameObject objNuvem, SpriteRenderer sr)
    {
        float diametroFinal = raioToxina * 2f;
        Vector3 escalaInicial = Vector3.one * (diametroFinal * 0.2f);
        Vector3 escalaFinal = Vector3.one * diametroFinal;

        float tempo = 0f;
        while (tempo < duracaoVisualNuvem)
        {
            if (objNuvem == null) yield break;

            tempo += Time.deltaTime;
            float t = tempo / duracaoVisualNuvem;

            float tCurva = 1f - Mathf.Pow(1f - t, 3f);
            objNuvem.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tCurva);

            float alpha = Mathf.Lerp(corNuvemToxica.a, 0f, t * t);
            sr.color = new Color(corNuvemToxica.r, corNuvemToxica.g, corNuvemToxica.b, alpha);

            yield return null;
        }

        if (objNuvem != null) Destroy(objNuvem);
    }

    private static Sprite ObterSpriteNuvem()
    {
        if (spriteNuvemCache != null) return spriteNuvemCache;

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
                    float normDist = dist / raio;
                    float ruido = Mathf.PerlinNoise(x * 0.08f, y * 0.08f);
                    float bordaGasosa = Mathf.Clamp01((1f - normDist) * (0.65f + ruido * 0.7f));
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, bordaGasosa));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();
        spriteNuvemCache = Sprite.Create(tex, new Rect(0, 0, tamanho, tamanho), new Vector2(0.5f, 0.5f), tamanho);
        return spriteNuvemCache;
    }

    private IEnumerator AtordoarComponente(MonoBehaviour tropa, float tempo)
    {
        if (tropa == null || !tropa.enabled) yield break;
        tropa.enabled = false;
        yield return new WaitForSeconds(tempo);
        if (tropa != null) tropa.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, raioToxina);
    }
}