using System.Collections;
using UnityEngine;

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

    // Propriedade pública para permitir a leitura por outros scripts (ex: BossStaphylococcus)
    public Transform[] PontosDeCaminho => pontosDeCaminho;

    [Header("Tempo entre ondas")]
    [SerializeField] private float tempoEntreOndas = 5f;
    [SerializeField] private float tempoInicioJogo = 2f;

    private int indiceOndaAtual = 0;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

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
        yield return new WaitForSeconds(tempoInicioJogo);

        while (indiceOndaAtual < ondas.Length)
        {
            yield return StartCoroutine(GerarOnda(ondas[indiceOndaAtual]));

            indiceOndaAtual++;

            if (indiceOndaAtual < ondas.Length)
            {
                yield return new WaitForSeconds(tempoEntreOndas);
            }
        }

        if (gameManager != null)
        {
            gameManager.AtivarChecagemVitoria();
        }
    }

    private IEnumerator GerarOnda(Onda onda)
    {
        if (onda.prefabsInimigos == null || onda.prefabsInimigos.Length == 0)
        {
            Debug.LogError($"[GerenciadorOndas] A Onda {indiceOndaAtual + 1} não tem prefabs de inimigos atribuídos!", this);
            yield break;
        }

        for (int i = 0; i < onda.quantidade; i++)
        {
            GameObject prefabAleatorio = onda.prefabsInimigos[Random.Range(0, onda.prefabsInimigos.Length)];
            
            SpawnarInimigo(prefabAleatorio);
            yield return new WaitForSeconds(onda.intervalo);
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