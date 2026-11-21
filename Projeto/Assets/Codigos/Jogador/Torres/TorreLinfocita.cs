using System.Collections;
using UnityEngine;

public class TorreLinfocita : TorreBase
{
    [Header("Configurações do Linfócito")]
    public float alcance = 3f;
    public float taxaAtaque = 1f;
    public float dano = 10f;
    public LayerMask layerInimigos;

    [Header("Tipo de Dano (Biológico)")]
    [Tooltip("Escolha o tipo de dano que esta torre causa aos inimigos")]
    public TipoDano tipoDano = TipoDano.Fagocito;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;
    public Animator animadorAtirador;

    [Header("Configurações de Animação")]
    public float duracaoAnimacaoAtaque = 0.3f;

    [Header("Projétil")]
    public GameObject prefabProjetil;
    public Transform pontoDisparo;

    private float tempoProximoAtaque = 0f;
    private Inimigo alvoAtual;
    private Coroutine corrotinaAtaque;

    private readonly Collider2D[] bufferInimigos = new Collider2D[20];
    private float proximoTempoBusca = 0f;

    public override float ObterRaioAlcance() => alcance;

    void Update()
    {
        if (emConstrucao) return;

        BuscarAlvoNoAlcance();

        bool temAlvo = (alvoAtual != null);
        AtualizarAnimacoesEstado(temAlvo);

        if (temAlvo && Time.time >= tempoProximoAtaque)
        {
            Atacar();
        }
    }

    void BuscarAlvoNoAlcance()
    {
        if (alvoAtual != null)
        {
            if (alvoAtual.EstaMorto || Vector2.Distance(transform.position, alvoAtual.transform.position) > alcance)
            {
                alvoAtual = null;
            }
        }

        if (Time.time < proximoTempoBusca) return;
        proximoTempoBusca = Time.time + 0.15f;

        int quantidade = layerInimigos != 0
            ? Physics2D.OverlapCircleNonAlloc(transform.position, alcance, bufferInimigos, layerInimigos)
            : Physics2D.OverlapCircleNonAlloc(transform.position, alcance, bufferInimigos);

        float menorDistanciaSqr = Mathf.Infinity;
        Inimigo melhorAlvo = null;

        for (int i = 0; i < quantidade; i++)
        {
            Inimigo inimigo = bufferInimigos[i].GetComponentInParent<Inimigo>();
            if (inimigo != null && !inimigo.EstaMorto)
            {
                float distanciaSqr = ((Vector2)inimigo.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distanciaSqr < menorDistanciaSqr)
                {
                    menorDistanciaSqr = distanciaSqr;
                    melhorAlvo = inimigo;
                }
            }
        }

        alvoAtual = melhorAlvo;
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
            projetilNormal.tipoDano = this.tipoDano;
            projetilNormal.DefinirAlvo(alvoAtual);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}