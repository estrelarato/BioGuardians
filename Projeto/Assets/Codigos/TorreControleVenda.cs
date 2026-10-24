using UnityEngine;

public class TorreControleVenda : MonoBehaviour
{
    [Header("Economia")]
    public int valorVenda = 50; // Quanto o jogador recebe ao vender

    // Chamado automaticamente pelo Unity ao clicar com o mouse sobre o Collider2D da torre
    void OnMouseDown()
    {
        // Encontra o gerenciador do menu na tela e abre o painel para ESTA torre
        MenuTorreUI menuUI = FindFirstObjectByType<MenuTorreUI>();
        if (menuUI != null)
        {
            menuUI.AbrirMenu(this);
        }
    }
}