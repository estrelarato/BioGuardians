using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreMastocita : TorreBase
{
    [Header("Configurações Gerais")]
    public float alcance = 3.5f;
    public float taxaAtaque = 1f;

    [Header("Configurações de Desaceleração")]
    [Range(0.1f, 0.9f)] public float porcentagemLento = 0.4f;
    public float duracaoLento = 2f;

    [Header("Animadores Independentes")]
    public Animator animadorTorre;

    [Header("Configurações de Carga e Disparo")]
    public float tempoCarga = 0.5f;
    public Vector3 escalaInicialTiro = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 escalaFinalTiro = Vector3.one;

    [Header("Disparo / Projétil")]
    public GameObject prefabMagia;
    public Transform pontoDisparo;

    private float tempoProximoAtaque = 0f;
    private List<Inimigo> inimigosNoAlcance = new List<Inimigo>();
    private Inimigo alvoAtual;
    private bool estaCarregandoTiro = false;

    // Referências locais da magia que está a ser carregada
    private GameObject magiaObjAtual;
    private Magia magiaAtual;
    private Coroutine corrotinaCarga;

    void Update()
    {
        if (emConstrucao) return;

        LimparLista();
        EscolherAlvo();

        bool temAlvo = (alvoAtual != null);
        AtualizarAnimacoesEstado(temAlvo);

        // Inicia o carregamento quando o tempo do cooldown permitir e houver alvo
        if (temAlvo && Time.time >= tempoProximoAtaque && !estaCarregandoTiro)
        {
            IniciarCarga();
        }
    }

    void IniciarCarga()
    {
        estaCarregandoTiro = true;
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

    // ========================================================================
    // CHAME ESTE MÉTODO VIA ANIMATION EVENT NO FRAME EXATO DO DISPARO
    // ========================================================================
    public void DispararMagia()
    {
        if (!estaCarregandoTiro) return;

        DefinirBoolCarregando(false);

        if (magiaObjAtual != null)
        {
            magiaObjAtual.transform.SetParent(null); // Desvincula do ponto de disparo
            if (magiaAtual != null)
            {
                magiaAtual.LiberarTiro();
            }
        }

        // Aplica efeito no alvo se ainda existir
        if (alvoAtual != null)
        {
            EfeitoLento efeito = alvoAtual.GetComponent<EfeitoLento>();
            if (efeito == null)
            {
                efeito = alvoAtual.gameObject.AddComponent<EfeitoLento>();
            }
            efeito.AplicarLentidao(porcentagemLento, duracaoLento);
        }

        // Reseta o estado para o próximo ataque respeitando a taxa de ataque
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Inimigo inimigo = other.GetComponentInParent<Inimigo>();
        if (inimigo != null && !inimigosNoAlcance.Contains(inimigo))
        {
            inimigosNoAlcance.Add(inimigo);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Inimigo inimigo = other.GetComponentInParent<Inimigo>();
        if (inimigo != null)
        {
            inimigosNoAlcance.Remove(inimigo);
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