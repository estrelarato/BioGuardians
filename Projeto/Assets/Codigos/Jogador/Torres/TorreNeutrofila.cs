using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreNeutrofila : TorreBase
{
    [Header("Configurações Gerais")]
    public float tempoEntreSpawns = 3f;
    public int limiteTropasVivas = 3;

    [Header("Spawn")]
    public GameObject prefabTropa;
    public Transform pontoSpawn;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;

    [Header("Configurações de Animação")]
    public float duracaoAnimacaoSpawn = 0.3f;

    private float tempoProximoSpawn = 0f;
    private Coroutine corrotinaSpawn;

    private List<GameObject> minhasTropas = new List<GameObject>();

    void Update()
    {
        if (emConstrucao) return;

        LimparListaTropas();

        if (Time.time >= tempoProximoSpawn)
        {
            if (PodeSpawnar())
            {
                GerarTropa();
            }
        }
    }

    void LimparListaTropas()
    {
        minhasTropas.RemoveAll(tropa => tropa == null);
    }

    bool PodeSpawnar()
    {
        return minhasTropas.Count < limiteTropasVivas;
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
            GameObject novaTropa = Instantiate(prefabTropa, pontoSpawn.position, Quaternion.identity);
            minhasTropas.Add(novaTropa);
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