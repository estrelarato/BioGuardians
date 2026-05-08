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

            yield return new WaitForSeconds(tempoEntreOndas);
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
        GameObject inimigoObj = Instantiate(prefab, pontoSpawn.position, Quaternion.identity);

        MovimentoInimigo movimento = inimigoObj.GetComponent<MovimentoInimigo>();

        if (movimento != null)
        {
            movimento.DefinirCaminho(pontosDeCaminho);
        }
    }
}