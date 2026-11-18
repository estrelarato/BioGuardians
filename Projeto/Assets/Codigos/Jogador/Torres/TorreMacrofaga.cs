using System.Collections;
using UnityEngine;

public class TorreMacrofaga : TorreBase
{
    [Header("Configurações Gerais")]
    public float alcance = 2.5f;
    public float taxaAtaque = 0.8f;
    public float dano = 30f;

    [Header("Tipo de Dano (Biológico)")]
    [Tooltip("Escolha o tipo de dano em área do Macrófago")]
    public TipoDano tipoDano = TipoDano.Fagocito;

    [Header("Configurações do Ataque no Chão")]
    public float raioDanoChao = 2f;
    public LayerMask layerInimigos;

    [Header("Efeito Visual da Área de Dano")]
    [Tooltip("Cor que pinta toda a área de dano no instante do golpe")]
    public Color corAreaDano = new Color(1f, 0.15f, 0.15f, 0.45f);
    [Tooltip("Quanto tempo a área fica vermelha desaparecendo após o golpe")]
    public float duracaoFlashArea = 0.35f;
    [Tooltip("Ordem de renderização (sortingOrder) para aparecer acima do chão e abaixo dos personagens")]
    public int sortingOrderArea = 2;
    [Tooltip("Opcional: Prefab extra de partículas/onda de choque")]
    public GameObject prefabEfeitoAtaque;
    public Transform pontoOrigemEfeito;
    public float duracaoEfeito = 1.5f;

    [Header("Animador")]
    public Animator animadorTorre;
    public string nomeTriggerAtacar = "Atacar";

    private float tempoProximoAtaque = 0f;
    private bool estaExecutandoAtaque = false;
    private MeshRenderer rendererAreaVermelha;
    private Material materialAreaVermelha;
    private Coroutine corrotinaFlash;

    public override float ObterRaioAlcance() => alcance;

    private void Reset()
    {
        custo = 75;
        emConstrucao = true;
    }

    void Awake()
    {
        CriarDiscoAreaDano();
    }

    void Update()
    {
        if (emConstrucao) return;

        bool temInimigoPerto = ChecarInimigosNoAlcance();

        if (temInimigoPerto && Time.time >= tempoProximoAtaque && !estaExecutandoAtaque)
        {
            IniciarAnimacaoAtaque();
        }
    }

    bool ChecarInimigosNoAlcance()
    {
        Collider2D inimigo = Physics2D.OverlapCircle(transform.position, alcance, layerInimigos);
        return inimigo != null;
    }

    void IniciarAnimacaoAtaque()
    {
        estaExecutandoAtaque = true;
        tempoProximoAtaque = Time.time + (1f / taxaAtaque);

        if (animadorTorre != null && !string.IsNullOrEmpty(nomeTriggerAtacar))
        {
            animadorTorre.ResetTrigger(nomeTriggerAtacar);
            animadorTorre.SetTrigger(nomeTriggerAtacar);
        }
        else
        {
            ExecutarImpactoChao();
            FinalizarAnimacaoAtaque();
        }
    }

    // ========================================================================
    // CHAMADO PELO ANIMATION EVENT NO FRAME EXATO DO DANO
    // ========================================================================
    public void ExecutarImpactoChao()
    {
        if (!estaExecutandoAtaque && animadorTorre != null) return;

        // 1. Pinta toda a área de dano de vermelho instantaneamente
        if (corrotinaFlash != null) StopCoroutine(corrotinaFlash);
        corrotinaFlash = StartCoroutine(PiscarAreaVermelha());

        // 2. Instancia prefab extra opcional, se houver
        if (prefabEfeitoAtaque != null)
        {
            Vector3 posicaoSpawn = pontoOrigemEfeito != null ? pontoOrigemEfeito.position : transform.position;
            GameObject efeitoObj = Instantiate(prefabEfeitoAtaque, posicaoSpawn, Quaternion.identity);

            if (duracaoEfeito > 0f)
            {
                Destroy(efeitoObj, duracaoEfeito);
            }
        }

        // 3. Aplica o dano nos inimigos dentro de raioDanoChao
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(transform.position, raioDanoChao, layerInimigos);
        foreach (Collider2D col in inimigosAtingidos)
        {
            Inimigo inimigo = col.GetComponentInParent<Inimigo>();
            if (inimigo != null)
            {
                inimigo.ReceberDano(dano, tipoDano);
            }
        }

        estaExecutandoAtaque = false;
    }

    public void FinalizarAnimacaoAtaque()
    {
        estaExecutandoAtaque = false;
    }

    private void CriarDiscoAreaDano()
    {
        GameObject discoObj = new GameObject("AreaDanoChaoVisual");
        discoObj.transform.SetParent(transform, false);
        discoObj.transform.localPosition = Vector3.zero;

        MeshFilter meshFilter = discoObj.AddComponent<MeshFilter>();
        rendererAreaVermelha = discoObj.AddComponent<MeshRenderer>();

        materialAreaVermelha = new Material(Shader.Find("Sprites/Default"));
        materialAreaVermelha.color = Color.clear;
        rendererAreaVermelha.material = materialAreaVermelha;
        rendererAreaVermelha.sortingOrder = sortingOrderArea;

        // Compensa a escala do objeto pai para o disco ter exatamente o raioDanoChao no mundo
        float escalaPai = Mathf.Abs(transform.lossyScale.x);
        if (escalaPai < 0.001f) escalaPai = 1f;
        float raioLocal = raioDanoChao / escalaPai;

        meshFilter.mesh = GerarMeshDisco(raioLocal, 48);
        rendererAreaVermelha.enabled = false;
    }

    private Mesh GerarMeshDisco(float raio, int segmentos)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segmentos + 1];
        int[] triangulos = new int[segmentos * 3];

        vertices[0] = Vector3.zero;
        float passoAngulo = (2f * Mathf.PI) / segmentos;

        for (int i = 0; i < segmentos; i++)
        {
            float angulo = i * passoAngulo;
            vertices[i + 1] = new Vector3(Mathf.Cos(angulo) * raio, Mathf.Sin(angulo) * raio, 0f);

            int idxTri = i * 3;
            triangulos[idxTri] = 0;
            triangulos[idxTri + 1] = (i + 1 == segmentos) ? 1 : i + 2;
            triangulos[idxTri + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangulos;
        mesh.RecalculateNormals();
        return mesh;
    }

    private IEnumerator PiscarAreaVermelha()
    {
        if (rendererAreaVermelha == null || materialAreaVermelha == null) yield break;

        rendererAreaVermelha.enabled = true;
        materialAreaVermelha.color = corAreaDano;

        float tempo = 0f;
        while (tempo < duracaoFlashArea)
        {
            tempo += Time.deltaTime;
            float alpha = Mathf.Lerp(corAreaDano.a, 0f, tempo / duracaoFlashArea);
            materialAreaVermelha.color = new Color(corAreaDano.r, corAreaDano.g, corAreaDano.b, alpha);
            yield return null;
        }

        rendererAreaVermelha.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alcance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDanoChao);
    }
}