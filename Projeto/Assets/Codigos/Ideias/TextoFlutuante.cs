using TMPro;
using UnityEngine;

public class TextoFlutuante : MonoBehaviour
{
    [Header("Configurações de Movimento e Tempo")]
    public float velocidadeSubida = 1.2f;
    public float tempoVida = 0.8f;
    public float deslocamentoAleatorioX = 0.25f;

    private TextMeshPro textoMesh;
    private Color corInicial;
    private float timer;
    private Vector3 direcaoMovimento;

    void Awake()
    {
        textoMesh = GetComponent<TextMeshPro>();
    }

    public void Configurar(string mensagem, Color cor, float tamanhoFonte = 4f)
    {
        if (textoMesh == null) textoMesh = GetComponent<TextMeshPro>();

        textoMesh.text = mensagem;
        textoMesh.color = cor;
        textoMesh.fontSize = tamanhoFonte;
        corInicial = cor;
        timer = tempoVida;

        // Pequeno desvio horizontal aleatório para os números não ficarem todos encavalados
        float offsetX = Random.Range(-deslocamentoAleatorioX, deslocamentoAleatorioX);
        direcaoMovimento = new Vector3(offsetX, 1f, 0f).normalized;
    }

    void Update()
    {
        // Sobe no espaço do mundo
        transform.position += direcaoMovimento * velocidadeSubida * Time.deltaTime;

        timer -= Time.deltaTime;

        // Fade out na segunda metade do tempo de vida
        if (textoMesh != null && timer < tempoVida * 0.5f)
        {
            float alpha = Mathf.Clamp01(timer / (tempoVida * 0.5f));
            textoMesh.color = new Color(corInicial.r, corInicial.g, corInicial.b, alpha);
        }

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}