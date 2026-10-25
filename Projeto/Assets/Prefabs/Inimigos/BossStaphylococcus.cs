using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStaphylococcus : Inimigo
{
    public enum TipoDano { Tipo1, Tipo2, Verdadeiro }

    [Header("Mecânica: Resistências")]
    [Range(0f, 1f)] public float resistenciaTipo1 = 0.70f;
    [Range(0f, 1f)] public float resistenciaTipo2 = 0.35f;

    [Header("Mecânica: Barreira de Sangue Coagulado")]
    public float vidaBarreiraMaxima = 150f;
    public float vidaBarreiraAtual;
    public float intervaloCriarBarreira = 12f;
    public GameObject objetoVisualBarreira;
    private bool barreiraAtiva = false;
    private float tempoProximaBarreira;

    [Header("Mecânica: Divisão ao Morrer")]
    public GameObject prefabSAureusComum;
    public int quantidadeMinionsAoMorrer = 5;

    [Header("Configurações da Aura de Desaceleração")]
    public float raioEscudo = 4f;
    public float FatorDesaceleracaoTorre = 0.5f;
    public GameObject objetoVisualEscudo;
    public LayerMask layerTorres;

    [Header("Configurações de Invocação Passiva (Minions)")]
    public GameObject prefabMinion;
    public int quantidadeMinionsPorInvocacao = 3;

    private bool auraAtiva = false;
    private float proximaPorcentagemInvocacao = 0.75f;
    private Dictionary<MonoBehaviour, float> taxasAtaqueOriginais = new Dictionary<MonoBehaviour, float>();
    private Dictionary<TorreN, float> temposSpawnOriginais = new Dictionary<TorreN, float>();

    void Awake()
    {
        vidaAtual = vidaMaxima;
        vidaBarreiraAtual = 0f;
    }

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (barraDeVida == null) barraDeVida = GetComponentInChildren<BarraDeVidaInimigo>();

        posicaoAnteriorX = transform.position.x;

        if (barraDeVida != null) barraDeVida.AtualizarBarra();

        tempoProximaBarreira = Time.time + intervaloCriarBarreira;
    }

    protected override void Update()
    {
        base.Update();

        if (!estaVivo) return;

        ChecarHabilidadesPorVida();
        GerenciarBarreiraCoagulo();

        if (auraAtiva)
        {
            AplicarEfeitoDesaceleracaoTorres();
        }
    }

    #region Mecânica: Barreira e Dano

    void GerenciarBarreiraCoagulo()
    {
        if (!barreiraAtiva && Time.time >= tempoProximaBarreira)
        {
            AtivarBarreiraCoagulo();
        }
    }

    void AtivarBarreiraCoagulo()
    {
        barreiraAtiva = true;
        vidaBarreiraAtual = vidaBarreiraMaxima;

        if (objetoVisualBarreira != null)
        {
            objetoVisualBarreira.SetActive(true);
        }
    }

    public void ReceberDanoComTipo(float quantidade, TipoDano tipo)
    {
        if (!estaVivo) return;

        float danoCalculado = quantidade;
        if (tipo == TipoDano.Tipo1) danoCalculado *= (1f - resistenciaTipo1);
        else if (tipo == TipoDano.Tipo2) danoCalculado *= (1f - resistenciaTipo2);

        float danoFinal = Mathf.Max(danoCalculado - defesa, 0);

        if (barreiraAtiva)
        {
            if (danoFinal >= vidaBarreiraAtual)
            {
                danoFinal -= vidaBarreiraAtual;
                vidaBarreiraAtual = 0;
                barreiraAtiva = false;

                if (objetoVisualBarreira != null) objetoVisualBarreira.SetActive(false);
                tempoProximaBarreira = Time.time + intervaloCriarBarreira;
            }
            else
            {
                vidaBarreiraAtual -= danoFinal;
                danoFinal = 0;
            }
        }

        if (danoFinal > 0)
        {
            vidaAtual -= danoFinal;

            if (barraDeVida != null) barraDeVida.AtualizarBarra();

            if (vidaAtual <= 0)
            {
                estaVivo = false;

                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null) gameManager.GanharDinheiro(recompensaMoedas);

                DividirEMorrer();
            }
        }
    }

    #endregion

    #region Mecânica: Divisão ao Morrer (Corrigido Ponto de Caminho)

    void DividirEMorrer()
    {
        RestaurarTorresForaDoAlcance(new HashSet<MonoBehaviour>());

        if (prefabSAureusComum != null)
        {
            Transform[] pontosCaminho = ObterPontosCaminho();

            for (int i = 0; i < quantidadeMinionsAoMorrer; i++)
            {
                Vector3 posicaoSpawn = transform.position + new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), 0);
                GameObject minionObj = Instantiate(prefabSAureusComum, posicaoSpawn, Quaternion.identity);

                Inimigo minionScript = minionObj.GetComponent<Inimigo>();
                if (minionScript != null)
                {
                    // Passa o caminho e o índice do ponto atual em que o Boss morreu!
                    minionScript.DefinirCaminho(pontosCaminho, indiceCaminhoAtual);
                }
            }
        }

        Destroy(gameObject);
    }

    #endregion

    #region Invocação e Aura (Corrigido Ponto de Caminho)

    void ChecarHabilidadesPorVida()
    {
        float porcentagemVidaAtual = vidaAtual / vidaMaxima;

        if (!auraAtiva && porcentagemVidaAtual <= 0.50f)
        {
            AtivarAura();
        }

        if (porcentagemVidaAtual <= proximaPorcentagemInvocacao && proximaPorcentagemInvocacao > 0f)
        {
            InvocarmInions();
            proximaPorcentagemInvocacao -= 0.25f;
        }
    }

    void AtivarAura()
    {
        auraAtiva = true;
        if (objetoVisualEscudo != null) objetoVisualEscudo.SetActive(true);
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
                // Passa o caminho e o índice do ponto atual
                minionScript.DefinirCaminho(ObterPontosCaminho(), indiceCaminhoAtual);
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
        return caminhos;
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

        foreach (var t in torresParaRemover) taxasAtaqueOriginais.Remove(t);

        List<TorreN> torresNParaRemover = new List<TorreN>();
        foreach (var par in temposSpawnOriginais)
        {
            if (!torresAtuais.Contains(par.Key) || par.Key == null)
            {
                if (par.Key != null) par.Key.tempoEntreSpawns = par.Value;
                torresNParaRemover.Add(par.Key);
            }
        }

        foreach (var tn in torresNParaRemover) temposSpawnOriginais.Remove(tn);
    }

    #endregion

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