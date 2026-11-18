using UnityEngine;

public abstract class TorreBase : MonoBehaviour
{
    [Header("Configurações Gerais de Construção")]
    public int custo = 50;
    public bool emConstrucao = true;

    [Header("Visualização de Alcance")]
    [Tooltip("Exibe o círculo de alcance enquanto a torre estiver sendo selecionada/construída")]
    public bool mostrarAlcanceNaConstrucao = true;
    public Color corCirculoAlcance = new Color(0.2f, 0.8f, 1f, 0.75f);
    public float larguraLinhaAlcance = 0.06f;
    [Range(24, 96)] public int segmentosCirculo = 48;

    private LineRenderer lineRendererAlcance;
    private float ultimoRaioDesenhado = -1f;

    public virtual float ObterRaioAlcance()
    {
        return 0f;
    }

    protected virtual void LateUpdate()
    {
        AtualizarCirculoAlcance();
    }

    private void AtualizarCirculoAlcance()
    {
        float raioMundo = ObterRaioAlcance();
        bool deveMostrar = mostrarAlcanceNaConstrucao && emConstrucao && raioMundo > 0f;

        if (!deveMostrar)
        {
            if (lineRendererAlcance != null && lineRendererAlcance.enabled)
            {
                lineRendererAlcance.enabled = false;
            }
            return;
        }

        if (lineRendererAlcance == null)
        {
            CriarLineRendererAlcance();
        }

        lineRendererAlcance.enabled = true;

        // Compensa a escala do objeto pai para o círculo ter o tamanho exato do mundo
        float escalaPai = Mathf.Abs(transform.lossyScale.x);
        if (escalaPai < 0.001f) escalaPai = 1f;
        float raioLocal = raioMundo / escalaPai;

        if (!Mathf.Approximately(ultimoRaioDesenhado, raioLocal))
        {
            DesenharVerticesCirculo(raioLocal);
            ultimoRaioDesenhado = raioLocal;
        }
    }

    private void CriarLineRendererAlcance()
    {
        GameObject objCirculo = new GameObject("CirculoAlcance");
        objCirculo.transform.SetParent(transform, false);
        objCirculo.transform.localPosition = Vector3.zero;

        lineRendererAlcance = objCirculo.AddComponent<LineRenderer>();
        lineRendererAlcance.useWorldSpace = false;
        lineRendererAlcance.loop = true;
        lineRendererAlcance.startWidth = larguraLinhaAlcance;
        lineRendererAlcance.endWidth = larguraLinhaAlcance;
        lineRendererAlcance.material = new Material(Shader.Find("Sprites/Default"));
        lineRendererAlcance.startColor = corCirculoAlcance;
        lineRendererAlcance.endColor = corCirculoAlcance;
        lineRendererAlcance.sortingOrder = 10;
    }

    private void DesenharVerticesCirculo(float raio)
    {
        lineRendererAlcance.positionCount = segmentosCirculo;
        float passoAngulo = (2f * Mathf.PI) / segmentosCirculo;

        for (int i = 0; i < segmentosCirculo; i++)
        {
            float angulo = i * passoAngulo;
            float x = Mathf.Cos(angulo) * raio;
            float y = Mathf.Sin(angulo) * raio;
            lineRendererAlcance.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}