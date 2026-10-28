using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreMacrofaga : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float alcance = 2.5f;
    public float taxaAtaque = 0.8f;
    public float dano = 30f;
    public int custo = 75;
    public bool emConstrucao = true;

    [Header("Tipo de Dano (Biológico)")]
    [Tooltip("Escolha o tipo de dano em área do Macrófago")]
    public TipoDano tipoDano = TipoDano.Fagocito;

    [Header("Configurações do Ataque no Chão")]
    public float raioDanoChao = 2f;
    public LayerMask layerInimigos;

    [Header("Animador")]
    public Animator animadorTorre;

    private float tempoProximoAtaque = 0f;

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
                // Envia o dano junto com o TipoDano selecionado no Inspector
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