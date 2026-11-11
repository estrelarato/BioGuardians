using UnityEngine;

public class HemaciaZumbi : MonoBehaviour
{
    [Header("Configurações dos Filhotes (Plasmodium)")]
    public GameObject prefabProtozoarioFilhote;
    public int quantidadeFilhotes = 3;
    [Tooltip("Distância aleatória em que os filhotes vão espalhar ao nascer")]
    public float raioEspalhamento = 0.3f;

    private Inimigo scriptInimigo;
    private bool jaGerouFilhotes = false;

    void Awake()
    {
        scriptInimigo = GetComponent<Inimigo>();
    }

    void OnDestroy()
    {
        // Evita gerar filhotes se o objeto for destruído ao fechar o jogo ou trocar de cena
        if (!gameObject.scene.isLoaded) return;

        // Só gera os filhotes se a vida realmente chegou a zero (morte em combate)
        if (scriptInimigo != null && scriptInimigo.vidaAtual <= 0 && !jaGerouFilhotes)
        {
            jaGerouFilhotes = true;
            GerarFilhotes();
        }
    }

    void GerarFilhotes()
    {
        if (prefabProtozoarioFilhote == null)
        {
            Debug.LogWarning("[HEMÁCIA ZUMBI] Prefab do filhote não foi atribuído no Inspector!");
            return;
        }

        for (int i = 0; i < quantidadeFilhotes; i++)
        {
            // Cria uma pequena variação de posição para os filhotes não nascerem sobrepostos
            Vector3 offset = Random.insideUnitCircle * raioEspalhamento;
            Vector3 posicaoSpawn = transform.position + offset;

            GameObject novoFilhote = Instantiate(prefabProtozoarioFilhote, posicaoSpawn, Quaternion.identity);

            // Transfere o caminho (waypoints) E o waypoint atual para o filhote continuar de onde a mãe parou
            Inimigo scriptFilhote = novoFilhote.GetComponent<Inimigo>();
            if (scriptFilhote != null && scriptInimigo != null)
            {
                scriptFilhote.DefinirCaminho(scriptInimigo.pontosCaminho, scriptInimigo.IndicePontoAtual);
            }
        }
    }
}