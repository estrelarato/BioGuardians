using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStaphylococcus : Inimigo
{
    [Header("Configurações do Escudo (Aura)")]
    public float raioEscudo = 4f;
    public float FatorDesaceleracaoTorre = 0.5f;
    public GameObject objetoVisualEscudo;
    public LayerMask layerTorres;

    [Header("Configurações de Invocação (Minions)")]
    public GameObject prefabMinion;
    public int quantidadeMinionsPorInvocacao = 3;

    private bool escudoAtivo = false;
    private float proximaPorcentagemInvocacao = 0.75f;
    private Dictionary<MonoBehaviour, float> taxasAtaqueOriginais = new Dictionary<MonoBehaviour, float>();
    private Dictionary<TorreN, float> temposSpawnOriginais = new Dictionary<TorreN, float>();

    protected override void Update()
    {
        base.Update();

        ChecarHabilidadesPorVida();

        if (escudoAtivo)
        {
            AplicarEfeitoDesaceleracaoTorres();
        }
    }

    void ChecarHabilidadesPorVida()
    {
        float porcentagemVidaAtual = vidaAtual / vidaMaxima;

        if (!escudoAtivo && porcentagemVidaAtual <= 0.50f)
        {
            AtivarEscudo();
        }

        if (porcentagemVidaAtual <= proximaPorcentagemInvocacao && proximaPorcentagemInvocacao > 0f)
        {
            InvocarmInions();
            proximaPorcentagemInvocacao -= 0.25f;
        }
    }

    void AtivarEscudo()
    {
        escudoAtivo = true;
        if (objetoVisualEscudo != null)
        {
            objetoVisualEscudo.SetActive(true);
        }
    }

    void InvocarmInions()
    {
        if (prefabMinion == null) return;

        for (int i = 0; i < quantidadeMinionsPorInvocacao; i++)
        {
            Vector3 offset = Random.insideUnitCircle * 0.8f;
            GameObject minionObj = Instantiate(prefabMinion, transform.position + offset, Quaternion.identity);

            Inimigo minionScript = minionObj.GetComponent<Inimigo>();
            if (minionScript != null)
            {
                minionScript.DefinirCaminho(ObterPontosCaminho());
            }
        }
    }

    Transform[] ObterPontosCaminho()
    {
        GerenciadorOndas gerenciador = FindFirstObjectByType<GerenciadorOndas>();
        if (gerenciador != null)
        {
            return gerenciador.PontosDeCaminho;
        }
        return null;
    }

    void AplicarEfeitoDesaceleracaoTorres()
    {
        Collider2D[] torresAtingidas = Physics2D.OverlapCircleAll(transform.position, raioEscudo, layerTorres);
        HashSet<MonoBehaviour> torresAtuaisNoAlcance = new HashSet<MonoBehaviour>();

        foreach (Collider2D col in torresAtingidas)
        {
            Torre torrePadrao = col.GetComponent<Torre>();
            if (torrePadrao != null)
            {
                torresAtuaisNoAlcance.Add(torrePadrao);
                if (!taxasAtaqueOriginais.ContainsKey(torrePadrao))
                {
                    taxasAtaqueOriginais[torrePadrao] = torrePadrao.taxaAtaque;
                    torrePadrao.taxaAtaque *= FatorDesaceleracaoTorre;
                }
            }

            TorreMacrofaga torreMacrofaga = col.GetComponent<TorreMacrofaga>();
            if (torreMacrofaga != null)
            {
                torresAtuaisNoAlcance.Add(torreMacrofaga);
                if (!taxasAtaqueOriginais.ContainsKey(torreMacrofaga))
                {
                    taxasAtaqueOriginais[torreMacrofaga] = torreMacrofaga.taxaAtaque;
                    torreMacrofaga.taxaAtaque *= FatorDesaceleracaoTorre;
                }
            }

            TorreN torreN = col.GetComponent<TorreN>();
            if (torreN != null)
            {
                torresAtuaisNoAlcance.Add(torreN);
                if (!temposSpawnOriginais.ContainsKey(torreN))
                {
                    temposSpawnOriginais[torreN] = torreN.tempoEntreSpawns;
                    torreN.tempoEntreSpawns /= FatorDesaceleracaoTorre;
                }
            }
        }

        RestaurarTorresForaDoAlcance(torresAtuaisNoAlcance);
    }

    void RestaurarTorresForaDoAlcance(HashSet<MonoBehaviour> torresAtuais)
    {
        List<MonoBehaviour> torresParaRemover = new List<MonoBehaviour>();

        foreach (var par in taxasAtaqueOriginais)
        {
            if (!torresAtuais.Contains(par.Key) || par.Key == null)
            {
                if (par.Key is Torre t) t.taxaAtaque = par.Value;
                if (par.Key is TorreMacrofaga tm) tm.taxaAtaque = par.Value;
                torresParaRemover.Add(par.Key);
            }
        }

        foreach (var t in torresParaRemover)
        {
            taxasAtaqueOriginais.Remove(t);
        }

        List<TorreN> torresNParaRemover = new List<TorreN>();
        foreach (var par in temposSpawnOriginais)
        {
            if (!torresAtuais.Contains(par.Key) || par.Key == null)
            {
                if (par.Key != null) par.Key.tempoEntreSpawns = par.Value;
                torresNParaRemover.Add(par.Key);
            }
        }

        foreach (var tn in torresNParaRemover)
        {
            temposSpawnOriginais.Remove(tn);
        }
    }

    private void OnDestroy()
    {
        RestaurarTorresForaDoAlcance(new HashSet<MonoBehaviour>());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, raioEscudo);
    }
}