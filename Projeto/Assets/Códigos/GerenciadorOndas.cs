using System.Collections;
using UnityEngine;

[System.Serializable]
public class Onda
{
    public GameObject prefabInimigo;
    public int quantidade = 10;
    public float intervalo = 1f;
}

public class GerenciadorOndas : MonoBehaviour
{
    [Header("Configuração das Ondas")]
    public Onda[] ondas;

    [Header("Spawn")]
    public Transform pontoSpawn;

    [Header("Caminho")]
    public Transform[] pontosDeCaminho;

    [Header("Tempo entre ondas")]
    public float tempoEntreOndas = 5f;

    private int indiceOndaAtual = 0;

    void Start()
    {
        StartCoroutine(IniciarOndas());
    }

    IEnumerator IniciarOndas()
    {
        yield return new WaitForSeconds(2f);

        while (indiceOndaAtual < ondas.Length)
        {
            yield return StartCoroutine(GerarOnda(ondas[indiceOndaAtual]));

            indiceOndaAtual++;

            if (indiceOndaAtual < ondas.Length)
            {
                yield return new WaitForSeconds(tempoEntreOndas);
            }
        }

        // --- MUDANÇA AQUI ---
        // Avisa o Gerenciador de Jogo que todas as ondas já foram spawnadas
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AtivarChecagemVitoria();
        }
    }

    IEnumerator GerarOnda(Onda onda)
    {
        for (int i = 0; i < onda.quantidade; i++)
        {
            SpawnarInimigo(onda.prefabInimigo);
            yield return new WaitForSeconds(onda.intervalo);
        }
    }

    void SpawnarInimigo(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject inimigoObj = Instantiate(prefab, pontoSpawn.position, Quaternion.identity);

        Inimigo movimento = inimigoObj.GetComponent<Inimigo>();

        if (movimento != null)
        {
            movimento.DefinirCaminho(pontosDeCaminho);
        }
    }
}