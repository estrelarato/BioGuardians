using UnityEngine;

public class FeiticoCura : FeiticoBase
{
    [Header("Parâmetros de Cura")]
    public int quantidadeCuraBase = 5;

    protected override void ExecutarLogicaInicial()
    {
        // Cura não precisa de mira no mapa, aplica direto na base
        AplicarCuraNaBase();
    }

    protected override void AplicarEfeitoNoPonto(Vector3 posicaoMundo)
    {
        // Não utilizado para cura direta
    }

    private void AplicarCuraNaBase()
    {
        VidaDoJogador sistemaVida = FindFirstObjectByType<VidaDoJogador>();
        if (sistemaVida != null)
        {
            int vidaAtual = sistemaVida.GetVidaAtual();
            int vidaMax = sistemaVida.GetVidaMaxima();
            int novaVida = Mathf.Min(vidaAtual + quantidadeCuraBase, vidaMax);
            
            int vidaParaAdicionar = novaVida - vidaAtual;
            if (vidaParaAdicionar > 0)
            {
                sistemaVida.TomarDano(-vidaParaAdicionar);
            }

            Debug.Log($"[FEITIÇO CURA] Cura aplicada na base! +{vidaParaAdicionar} HP");
            IniciarCooldown();
        }
        else
        {
            // Reembolsa ATP se não encontrar a base no mapa
            if (GameManager.Instance != null)
                GameManager.Instance.GanharDinheiro(custoATP);
        }
    }
}