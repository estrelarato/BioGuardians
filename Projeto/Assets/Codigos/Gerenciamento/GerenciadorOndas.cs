using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Para a classe Button
using TMPro; // Para a classe TextMeshProUGUI

[System.Serializable]
public class Onda
{
    [Tooltip("Lista de prefabs de inimigos que podem surgir nesta onda.")]
    public GameObject[] prefabsInimigos;
    
    [Tooltip("Quantidade total de inimigos a gerar nesta onda.")]
    public int quantidade = 10;
    
    [Tooltip("Intervalo de tempo (em segundos) entre cada inimigo.")]
    public float intervalo = 1f;
}

public class GerenciadorOndas : MonoBehaviour
{
    [Header("Configuração das Ondas")]
    [SerializeField] private Onda[] ondas;

    [Header("Spawn")]
    [Tooltip("Pontos de onde os inimigos podem surgir.")]
    [SerializeField] private Transform[] pontosSpawn;

    [Header("Caminho")]
    [Tooltip("Waypoints que o inimigo deve seguir.")]
    [SerializeField] private Transform[] pontosDeCaminho;

    public Transform[] PontosDeCaminho => pontosDeCaminho;

    [Header("Tempo entre ondas")]
    [SerializeField] private float tempoEntreOndas = 10f;
    [SerializeField] private float tempoInicioJogo = 5f;

    [Header("UI de Contagem Regressiva e Skip")]
    [Tooltip("Arraste seu texto de TextMeshPro (TMP) aqui")]
    [SerializeField] private TextMeshProUGUI textoContagemTMP;
    
    [Tooltip("Arraste o seu Botão de Skip aqui")]
    [SerializeField] private Button botaoSkip;

    private int indiceOndaAtual = 0;
    private GameManager gameManager;
    private bool pularEspera = false;
    private float tempoRestante = 0f;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        // Conecta automaticamente a função de pular ao clique do botão
        if (botaoSkip != null)
        {
            botaoSkip.onClick.AddListener(PularContagem);
        }

        if (!ValidarConfiguracao()) return;

        StartCoroutine(IniciarOndas());
    }

    private bool ValidarConfiguracao()
    {
        if (ondas == null || ondas.Length == 0)
        {
            Debug.LogError("[GerenciadorOndas] Nenhuma onda configurada!", this);
            return false;
        }

        if (pontosSpawn == null || pontosSpawn.Length == 0)
        {
            Debug.LogError("[GerenciadorOndas] Nenhum Ponto de Spawn atribuído!", this);
            return false;
        }

        return true;
    }

    private IEnumerator IniciarOndas()
    {
        // Contagem regressiva inicial antes de começar a 1ª Onda
        yield return StartCoroutine(AguardarComContagem(tempoInicioJogo));

        while (indiceOndaAtual < ondas.Length)
        {
            // Oculta a UI e o Botão durante o spawn da onda
            AtualizarUIContagem(0, false);

            // Spawna todos os inimigos da onda
            yield return StartCoroutine(GerarOnda(ondas[indiceOndaAtual]));

            indiceOndaAtual++;

            // Se ainda restarem ondas, aguarda com contagem regressiva
            if (indiceOndaAtual < ondas.Length)
            {
                yield return StartCoroutine(AguardarComContagem(tempoEntreOndas));
            }
        }

        AtualizarUIContagem(0, false);

        if (gameManager != null)
        {
            gameManager.AtivarChecagemVitoria();
        }
    }

    private IEnumerator GerarOnda(Onda onda)
    {
        if (onda.prefabsInimigos == null || onda.prefabsInimigos.Length == 0)
        {
            Debug.LogError($"[GerenciadorOndas] A Onda {indiceOndaAtual + 1} não tem prefabs de inimigos!", this);
            yield break;
        }

        // Garante que o botão fique oculto enquanto os inimigos nascem
        if (botaoSkip != null) 
            botaoSkip.gameObject.SetActive(false);

        for (int i = 0; i < onda.quantidade; i++)
        {
            GameObject prefabAleatorio = onda.prefabsInimigos[Random.Range(0, onda.prefabsInimigos.Length)];
            
            SpawnarInimigo(prefabAleatorio);
            yield return new WaitForSeconds(onda.intervalo);
        }
    }

    private IEnumerator AguardarComContagem(float tempoTotal)
    {
        tempoRestante = tempoTotal;
        pularEspera = false;

        // Exibe a UI e o Botão de Skip quando o spawn terminar
        AtualizarUIContagem(tempoRestante, true);

        while (tempoRestante > 0 && !pularEspera)
        {
            tempoRestante -= Time.deltaTime;
            AtualizarUIContagem(tempoRestante, true);
            yield return null;
        }

        pularEspera = false;
        AtualizarUIContagem(0, false);
    }

    public void PularContagem()
    {
        pularEspera = true;
    }

    private void AtualizarUIContagem(float tempo, bool visivel)
    {
        if (botaoSkip != null) 
            botaoSkip.gameObject.SetActive(visivel);

        if (textoContagemTMP != null)
        {
            textoContagemTMP.gameObject.SetActive(visivel);
            if (visivel)
            {
                textoContagemTMP.text = $"{Mathf.CeilToInt(tempo)}";
            }
        }
    }

    private void SpawnarInimigo(GameObject prefab)
    {
        if (prefab == null) return;

        Transform spawnPoint = pontosSpawn[Random.Range(0, pontosSpawn.Length)];

        GameObject inimigoObj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        if (inimigoObj.TryGetComponent<Inimigo>(out Inimigo movimento))
        {
            movimento.DefinirCaminho(pontosDeCaminho);
        }
    }
}