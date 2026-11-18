using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreMacrofaga : TorreBase
{
    [Header("Configurações Gerais")]
    public float alcance = 2.5f;
    public float taxaAtaque = 0.8f;
    public float dano = 30f;
    // Removidos custo e emConstrucao daqui (eles vêm do TorreBase!)

    [Header("Tipo de Dano (Biológico)")]
    [Tooltip("Escolha o tipo de dano em área do Macrófago")]
    public TipoDano tipoDano = TipoDano.Fagocito;

    [Header("Configurações do Ataque no Chão")]
    public float raioDanoChao = 2f;
    public LayerMask layerInimigos;

    [Header("Animador")]
    public Animator animadorTorre;

    private float tempoProximoAtaque = 0f;

    // Define 75 como custo padrão no Inspector assim que você adiciona o script ao objeto
    private void Reset()
    {
        custo = 75;
        emConstrucao = true;
    }

    void Update()
    {
        if (emConstrucao) return;

        bool temInimigoPerto = ChecarInimigosNoAlcance();
        AtualizarAnimacoesEstado(temInimigoPerto);

        if (temInimigoPerto && Time.time >= tempoProximoAtaque)
        {
            AtacarChao();
        }
    }

    bool ChecarInimigosNoAlcance()
    {
        Collider2D inimigo = Physics2D.OverlapCircle(transform.position, alcance, layerInimigos);
        return inimigo != null;
    }

    void AtacarChao()
    {
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(transform.position, raioDanoChao, layerInimigos);
        foreach (Collider2D col in inimigosAtingidos)
        {
            Inimigo inimigo = col.GetComponent<Inimigo>();
            if (inimigo != null)
            {
                inimigo.ReceberDano(dano, tipoDano);
            }
        }

        tempoProximoAtaque = Time.time + 1f / taxaAtaque;
    }

    void AtualizarAnimacoesEstado(bool emAlerta)
    {
        if (animadorTorre != null) 
            animadorTorre.SetBool("EmAlerta", emAlerta);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alcance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDanoChao);
    }
}