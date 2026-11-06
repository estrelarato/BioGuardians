using UnityEngine;

public class TorreBaEo : TorreBase
{
    [Header("Prefabs das Tropas")]
    public GameObject prefabBasofila;
    public GameObject prefabEosinofila;

    [Header("Configurações da Área da Torre")]
    [Tooltip("Raio máximo que as tropas podem andar a partir desta torre")]
    public float raioAreaTorre = 5.0f;

    [Header("Pontos de Spawn e Rally")]
    public Transform pontoSpawn;
    public Transform pontoRally;

    private TropaBasofila basofilaAtual;
    private TropaEosinofila eosinofilaAtual;
    private bool foiConstruida = false;

    void Start()
    {
        if (pontoSpawn == null) pontoSpawn = transform;
        if (pontoRally == null) pontoRally = transform;
    }

    void Update()
    {
        if (emConstrucao) return;

        if (!foiConstruida)
        {
            foiConstruida = true;
            InicializarTropas();
        }
    }

    void InicializarTropas()
    {
        SpawnarBasofila();
        SpawnarEosinofila();
    }

    void SpawnarBasofila()
    {
        if (prefabBasofila == null) return;

        GameObject basoObj = Instantiate(prefabBasofila, pontoSpawn.position, Quaternion.identity);
        basofilaAtual = basoObj.GetComponent<TropaBasofila>();

        if (basofilaAtual != null)
        {
            basofilaAtual.ConfigurarTorreERally(transform.position, raioAreaTorre, pontoRally);
        }

        if (eosinofilaAtual != null && basofilaAtual != null)
        {
            eosinofilaAtual.ConfigurarAlvoBasofila(basofilaAtual);
        }
    }

    void SpawnarEosinofila()
    {
        if (prefabEosinofila == null) return;

        GameObject eosObj = Instantiate(prefabEosinofila, pontoSpawn.position, Quaternion.identity);
        eosinofilaAtual = eosObj.GetComponent<TropaEosinofila>();

        if (eosinofilaAtual != null)
        {
            eosinofilaAtual.ConfigurarTorre(transform.position, raioAreaTorre);
            
            if (basofilaAtual != null)
            {
                eosinofilaAtual.ConfigurarAlvoBasofila(basofilaAtual);
            }
        }
    }

    public void MudarPontoRally(Vector3 novaPosicao)
    {
        Vector3 vetorDistancia = novaPosicao - transform.position;
        if (vetorDistancia.magnitude > raioAreaTorre)
        {
            novaPosicao = transform.position + vetorDistancia.normalized * raioAreaTorre;
        }

        if (pontoRally != null)
        {
            pontoRally.position = novaPosicao;
        }
    }

    private void OnDestroy()
    {
        if (basofilaAtual != null) Destroy(basofilaAtual.gameObject);
        if (eosinofilaAtual != null) Destroy(eosinofilaAtual.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raioAreaTorre);
    }
}