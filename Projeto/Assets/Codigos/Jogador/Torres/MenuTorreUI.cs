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
        bool estaPausado = (PauseManager.Instancia != null && PauseManager.Instancia.EstaPausado);
        Debug.Log("[MENU] Entrou no AbrirMenu! Pause ativo? " + estaPausado);

        // Garante que nunca abra se o Pause ou Opções estiverem ativos
        if (estaPausado)
            return;

        torreSelecionada = torre;

        if (textoValorVenda != null)
        {
            textoValorVenda.text = "+" + torreSelecionada.valorVenda;
        }

        if (painelTorre != null)
        {
            painelTorre.SetActive(true);
            Debug.Log("[MENU] SetActive(true) executado! Ativo na hierarquia agora? " + painelTorre.activeInHierarchy);
        }
        else
        {
            Debug.LogError("[MENU] ERRO: O campo painelTorre está vazio (None)!");
        }
    }

    /// <summary>
    /// Esconde o menu de UI
    /// </summary>
    public void FecharMenu()
    {
        torreSelecionada = null;

        if (painelTorre != null)
        {
            painelTorre.SetActive(false);
        }
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