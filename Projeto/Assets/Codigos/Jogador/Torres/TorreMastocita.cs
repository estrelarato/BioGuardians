using System.Collections;
using UnityEngine;

public class TorreMastocita : TorreBase
{
    [Header("Configurações Gerais")]
    public float alcance = 3.5f;
    public float taxaAtaque = 1f;
    public LayerMask layerInimigos;

    [Header("Configurações de Desaceleração")]
    [Range(0.1f, 0.9f)] public float porcentagemLento = 0.4f;
    public float duracaoLento = 2f;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;

    [Header("Configurações de Carga e Disparo")]
    public float tempoCarga = 0.5f;
    public Vector3 escalaInicialTiro = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 escalaFinalTiro = Vector3.one;
    [Tooltip("Segurança: tempo máximo carregando antes de disparar automaticamente caso o Animation Event falhe")]
    public float tempoMaximoCargaSeguranca = 1.2f;

    [Header("Disparo / Projétil")]
    public GameObject prefabMagia;
    public Transform pontoDisparo;

    private float tempoProximoAtaque = 0f;
    private float tempoInicioCarga = 0f;
    private Inimigo alvoAtual;
    private bool estaCarregandoTiro = false;

    private GameObject magiaObjAtual;
    private Magia magiaAtual;
    private Coroutine corrotinaCarga;

    private readonly Collider2D[] bufferInimigos = new Collider2D[20];
    private float proximoTempoBusca = 0f;

    public override float ObterRaioAlcance() => alcance;

    void Update()
    {
        if (emConstrucao) return;

        BuscarAlvoNoAlcance();

        bool temAlvo = (alvoAtual != null);
        AtualizarAnimacoesEstado(temAlvo);

        if (estaCarregandoTiro && Time.time >= tempoInicioCarga + tempoMaximoCargaSeguranca)
        {
            DispararMagia();
            return;
        }

        if (temAlvo && Time.time >= tempoProximoAtaque && !estaCarregandoTiro)
        {
            IniciarCarga();
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

    void IniciarCarga()
    {
        estaCarregandoTiro = true;
        tempoInicioCarga = Time.time;
        DefinirBoolCarregando(true);

        if (prefabMagia != null && pontoDisparo != null)
        {
            magiaObjAtual = Instantiate(prefabMagia, pontoDisparo.position, Quaternion.identity, pontoDisparo);
            magiaObjAtual.transform.localScale = escalaInicialTiro;

            magiaAtual = magiaObjAtual.GetComponent<Magia>();
            if (magiaAtual != null && alvoAtual != null)
            {
                magiaAtual.DefinirAlvo(alvoAtual);
            }

            if (corrotinaCarga != null) StopCoroutine(corrotinaCarga);
            corrotinaCarga = StartCoroutine(CrescerMagia());
        }
        else if (animadorTorre == null)
        {
            DispararMagia();
        }
    }

    private IEnumerator CrescerMagia()
    {
        float tempoDecorrido = 0f;
        while (tempoDecorrido < tempoCarga)
        {
            tempoDecorrido += Time.deltaTime;
            float t = tempoDecorrido / tempoCarga;

            if (magiaObjAtual != null)
            {
                magiaObjAtual.transform.localScale = Vector3.Lerp(escalaInicialTiro, escalaFinalTiro, t);
            }

            yield return null;
        }
    }

    public void DispararMagia()
    {
        if (!estaCarregandoTiro) return;

        DefinirBoolCarregando(false);

        if (alvoAtual == null || alvoAtual.EstaMorto)
        {
            proximoTempoBusca = 0f;
            BuscarAlvoNoAlcance();
        }

        if (magiaObjAtual != null)
        {
            if (alvoAtual != null)
            {
                magiaObjAtual.transform.SetParent(null);
                if (magiaAtual != null)
                {
                    magiaAtual.DefinirAlvo(alvoAtual);
                    magiaAtual.LiberarTiro();
                }
            }
            else
            {
                Destroy(magiaObjAtual);
            }
        }

        if (alvoAtual != null)
        {
            EfeitoLento efeito = alvoAtual.GetComponent<EfeitoLento>();
            if (efeito == null)
            {
                efeito = alvoAtual.gameObject.AddComponent<EfeitoLento>();
            }
            efeito.AplicarLentidao(porcentagemLento, duracaoLento);
        }

        tempoProximoAtaque = Time.time + (1f / taxaAtaque);
        estaCarregandoTiro = false;
        magiaObjAtual = null;
        magiaAtual = null;
    }

    void DefinirBoolCarregando(bool carregando)
    {
        if (animadorTorre != null) animadorTorre.SetBool("estaCarregando", carregando);
    }

    void AtualizarAnimacoesEstado(bool emAlerta)
    {
        if (animadorTorre != null) animadorTorre.SetBool("EmAlerta", emAlerta);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}