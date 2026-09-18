using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreN : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float tempoEntreSpawns = 3f;
    public int limiteTropasVivas = 3;
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

    // Lista individual para esta torre controlar suas próprias tropas
    private List<GameObject> minhasTropas = new List<GameObject>();

    void Update()
    {
        if (emConstrucao) return;

        // Limpa referências de tropas que foram destruídas (explodiram/morreram)
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
        // Remove da lista os objetos que já foram destruídos da cena
        minhasTropas.RemoveAll(tropa => tropa == null);
    }

    bool PodeSpawnar()
    {
        // Verifica apenas a quantidade de tropas pertencentes a ESTA torre
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
            
            // Adiciona a nova tropa à lista exclusiva desta torre
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