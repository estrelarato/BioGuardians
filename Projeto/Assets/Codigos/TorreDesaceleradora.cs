using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreDesaceleradora : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float alcance = 3.5f;
    public float taxaAtaque = 1f;
    public int custo = 60;
    public bool emConstrucao = true;

    [Header("Configurações de Desaceleração")]
    [Range(0.1f, 0.9f)] public float porcentagemLento = 0.4f;
    public float duracaoLento = 2f;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;
    public Animator animadorAtirador;

    [Header("Configurações de Animação")]
    public float duracaoAnimacaoAtaque = 0.3f;

    [Header("Disparo / Projétil")]
    public GameObject prefabProjetil;
    public Transform pontoDisparo;

    private float tempoProximoAtaque = 0f;
    private List<Inimigo> inimigosNoAlcance = new List<Inimigo>();
    private Inimigo alvoAtual;
    private Coroutine corrotinaAtaque;

    void Update()
    {
        if (emConstrucao) return; // Se continuar travado em construção, nada abaixo executa

        LimparLista();
        EscolherAlvo();

        bool temAlvo = (alvoAtual != null);
        AtualizarAnimacoesEstado(temAlvo);

        if (temAlvo && Time.time >= tempoProximoAtaque)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        Debug.Log($"[TORRE LENTO] Atacando o alvo: {alvoAtual.name}");

        if (corrotinaAtaque != null) StopCoroutine(corrotinaAtaque);
        corrotinaAtaque = StartCoroutine(ControladorBoolAtaque());

        Disparar();
        tempoProximoAtaque = Time.time + 1f / taxaAtaque;
    }

    void Disparar()
    {
        if (alvoAtual == null) return;

        if (prefabProjetil != null && pontoDisparo != null)
        {
            GameObject projetilObj = Instantiate(prefabProjetil, pontoDisparo.position, Quaternion.identity);
            Projetil projetil = projetilObj.GetComponent<Projetil>();
            if (projetil != null)
            {
                projetil.DefinirAlvo(alvoAtual);
            }
        }

        EfeitoLento efeito = alvoAtual.GetComponent<EfeitoLento>();
        if (efeito == null)
        {
            efeito = alvoAtual.gameObject.AddComponent<EfeitoLento>();
        }

        efeito.AplicarLentidao(porcentagemLento, duracaoLento);
        Debug.Log($"[TORRE LENTO] Efeito de lentidão aplicado em {alvoAtual.name}!");
    }

    private IEnumerator ControladorBoolAtaque()
    {
        if (animadorTorre != null) animadorTorre.SetBool("EstaAtirando", true);
        if (animadorAtirador != null) animadorAtirador.SetBool("EstaAtirando", true);

        yield return new WaitForSeconds(duracaoAnimacaoAtaque);

        if (animadorTorre != null) animadorTorre.SetBool("EstaAtirando", false);
        if (animadorAtirador != null) animadorAtirador.SetBool("EstaAtirando", false);
    }

    void AtualizarAnimacoesEstado(bool emAlerta)
    {
        if (animadorTorre != null) animadorTorre.SetBool("EmAlerta", emAlerta);
        if (animadorAtirador != null) animadorAtirador.SetBool("EmAlerta", emAlerta);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[TORRE LENTO] Objeto entrou no raio: {other.name}");

        // Tenta pegar o Inimigo no próprio objeto ou nos pais dele
        Inimigo inimigo = other.GetComponentInParent<Inimigo>();
        if (inimigo != null && !inimigosNoAlcance.Contains(inimigo))
        {
            inimigosNoAlcance.Add(inimigo);
            Debug.Log($"[TORRE LENTO] Inimigo reconhecido e adicionado à lista: {inimigo.name}");
        }
        else if (inimigo == null)
        {
            Debug.LogWarning($"[TORRE LENTO] O objeto {other.name} entrou no raio, mas NÃO possui o script 'Inimigo'!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Inimigo inimigo = other.GetComponentInParent<Inimigo>();
        if (inimigo != null)
        {
            inimigosNoAlcance.Remove(inimigo);
            Debug.Log($"[TORRE LENTO] Inimigo saiu do alcance: {inimigo.name}");
        }
    }

    void LimparLista() => inimigosNoAlcance.RemoveAll(inimigo => inimigo == null);

    void EscolherAlvo()
    {
        float menorDistanciaSqr = Mathf.Infinity;
        Inimigo melhorAlvo = null;

        foreach (Inimigo inimigo in inimigosNoAlcance)
        {
            Vector2 direcao = inimigo.transform.position - transform.position;
            float distanciaSqr = direcao.sqrMagnitude;

            if (distanciaSqr < menorDistanciaSqr)
            {
                menorDistanciaSqr = distanciaSqr;
                melhorAlvo = inimigo;
            }
        }

        alvoAtual = melhorAlvo;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}