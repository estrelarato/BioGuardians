using UnityEngine;
using TMPro;

public class MenuTorreUI : MonoBehaviour
{
    [Header("Componentes de UI")]
    public GameObject painelTorre;
    public TextMeshProUGUI textoValorVenda;

    private TorreControleVenda torreSelecionada;

    /// <summary>
    /// Abre o menu com os dados da torre clicada
    /// </summary>
    public void AbrirMenu(TorreControleVenda torre)
    {
        torreSelecionada = torre;

        if (textoValorVenda != null)
        {
            textoValorVenda.text = "+" + torreSelecionada.valorVenda ;
        }

        painelTorre.SetActive(true);
    }

    /// <summary>
    /// Esconde o menu de UI
    /// </summary>
    public void FecharMenu()
    {
        torreSelecionada = null;
        painelTorre.SetActive(false);
    }

    /// <summary>
    /// Função para ligar no Botão de Vender na UI
    /// </summary>
    public void ConfirmarVenda()
    {
        if (torreSelecionada != null)
        {
            // Reembolsa o jogador no GameManager
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.GanharDinheiro(torreSelecionada.valorVenda);
            }

            // Destrói o objeto da torre do jogo
            Destroy(torreSelecionada.gameObject);

            // Esconde o painel após vender
            FecharMenu();
        }
    }
}