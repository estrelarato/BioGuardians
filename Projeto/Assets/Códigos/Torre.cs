using System.Collections.Generic;
using UnityEngine;

public class Torre : MonoBehaviour
{
    [Header("Configurações")]
    public float alcance = 3f;
    public float taxaAtaque = 1f;
    public float dano = 10f;
    public int custo = 50;
    public bool emConstrucao = true;

    [Header("Animadores")]
    public Animator animadorTorre;   
    public Animator animadorAtirador;

    [Header("Projétil")]
    public GameObject prefabProjetil;
    public Transform pontoDisparo;

    private float tempoProximoAtaque = 0f;
    private List<Inimigo> inimigosNoAlcance = new List<Inimigo>();
    private Inimigo alvoAtual;

    void Update()
    {
        if (emConstrucao)
            return;

        LimparLista();
        EscolherAlvo();

        bool temAlvo = (alvoAtual != null);
        AtualizarAnimacoesEstado(temAlvo);

        if (temAlvo)
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
        float menorDistancia = Mathf.Infinity;
        Inimigo melhorAlvo = null;

        foreach (Inimigo inimigo in inimigosNoAlcance)
        {
            float distancia = Vector2.Distance(transform.position, inimigo.transform.position);

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                melhorAlvo = inimigo;
            }
        }

        alvoAtual = melhorAlvo;
    }

    void Atacar()
    {
        if (Time.time >= tempoProximoAtaque)
        {
            if (animadorAtirador != null)
            {
                animadorAtirador.SetTrigger("Atacar");
            }

            Disparar();
            tempoProximoAtaque = Time.time + 1f / taxaAtaque;
        }
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
        }

        ProjetilExplosivo projetilArea = projetilObj.GetComponent<ProjetilExplosivo>();
        if (projetilArea != null)
        {
            projetilArea.dano = this.dano;
            projetilArea.DefinirAlvo(alvoAtual);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}