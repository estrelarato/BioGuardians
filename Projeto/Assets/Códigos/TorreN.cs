using System.Collections;
using UnityEngine;

public class TorreN : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float tempoEntreSpawns = 3f;
    public int limiteTropasVivas = 5;
    public int custo = 50;
    public bool emConstrucao = true;

    [Header("Spawn")]
    public GameObject prefabTropa;
    public Transform pontoSpawn;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;

    [Header("Configurações de Animação")]
    public float duracaoAnimacaoSpawn = 0.3f;

    private float tempoProximoSpawn = 0f;
    private Coroutine corrotinaSpawn;

    void Update()
    {
        if (emConstrucao) return;

        if (Time.time >= tempoProximoSpawn)
        {
            if (PodeSpawnar())
            {
                GerarTropa();
            }
        }
    }

    bool PodeSpawnar()
    {
        // Busca na cena pelo novo nome do componente TropaNeutrofila
        int tropasAtivas = FindObjectsOfType<TropaNeutrofila>().Length;
        return tropasAtivas < limiteTropasVivas;
    }

    void GerarTropa()
    {
        if (corrotinaSpawn != null)
        {
            StopCoroutine(corrotinaSpawn);
        }

        corrotinaSpawn = StartCoroutine(ControladorBoolSpawn());

        if (prefabTropa != null && pontoSpawn != null)
        {
            Instantiate(prefabTropa, pontoSpawn.position, Quaternion.identity);
        }

        tempoProximoSpawn = Time.time + tempoEntreSpawns;
    }

    private IEnumerator ControladorBoolSpawn()
    {
        if (animadorTorre != null) animadorTorre.SetBool("EmAlerta", true);

        yield return new WaitForSeconds(duracaoAnimacaoSpawn);

        if (animadorTorre != null) animadorTorre.SetBool("EmAlerta", false);

    }
}