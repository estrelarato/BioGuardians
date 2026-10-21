using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torre : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float alcance = 3f;
    public float taxaAtaque = 1f;
    public float dano = 10f;
    public int custo = 50;
    public bool emConstrucao = true;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;    // Base ou estrutura da torre
    public Animator animadorAtirador; // Personagem, canhão ou topo

    [Header("Configurações de Animação")]
    public float duracaoAnimacaoAtaque = 0.3f;

    [Header("Projétil")]
    public GameObject prefabProjetil;
    public Transform pontoDisparo;

    private float tempoProximoAtaque = 0f;
    private List<Inimigo> inimigosNoAlcance = new List<Inimigo>();
    private Inimigo alvoAtual;
    private Coroutine corrotinaAtaque;

    void Update()
    {
        if (emConstrucao) return;

        LimparLista();
        EscolherAlvo();

        bool temAlvo = (alvoAtual != null);
        AtualizarAnimacoesEstado(temAlvo);

        if (temAlvo && Time.time >= tempoProximoAtaque)
        {
            Atacar();
        }
    }

    void AtualizarAnimacoesEstado(bool emAlerta)
    {
        if (animadorTorre != null)
            animadorTorre.SetBool("EmAlerta", emAlerta);

        if (animadorAtirador != null)
            animadorAtirador.SetBool("EmAlerta", emAlerta);
    }

    void Atacar()
    {
        if (corrotinaAtaque != null)
        {
            StopCoroutine(corrotinaAtaque);
        }

        corrotinaAtaque = StartCoroutine(ControladorBoolAtaque());

        Disparar();
        tempoProximoAtaque = Time.time + 1f / taxaAtaque;
    }

    private IEnumerator ControladorBoolAtaque()
    {
        if (animadorTorre != null) animadorTorre.SetBool("EstaAtirando", true);
        if (animadorAtirador != null) animadorAtirador.SetBool("EstaAtirando", true);

        yield return new WaitForSeconds(duracaoAnimacaoAtaque);

        if (animadorTorre != null) animadorTorre.SetBool("EstaAtirando", false);
        if (animadorAtirador != null) animadorAtirador.SetBool("EstaAtirando", false);
    }

    void Disparar()
    {
        if (prefabProjetil == null || pontoDisparo == null || alvoAtual == null) return;

        GameObject projetilObj = Instantiate(prefabProjetil, pontoDisparo.position, Quaternion.identity);

        Projetil projetilNormal = projetilObj.GetComponent<Projetil>();
        if (projetilNormal != null)
        {
            projetilNormal.dano = this.dano;
            projetilNormal.DefinirAlvo(alvoAtual);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Inimigo inimigo = other.GetComponent<Inimigo>();
        if (inimigo != null && !inimigosNoAlcance.Contains(inimigo))
        {
            inimigosNoAlcance.Add(inimigo);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Inimigo inimigo = other.GetComponent<Inimigo>();
        if (inimigo != null)
        {
            inimigosNoAlcance.Remove(inimigo);
        }
    }

    void LimparLista()
    {
        inimigosNoAlcance.RemoveAll(inimigo => inimigo == null);
    }

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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}