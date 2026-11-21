using UnityEngine;
using UnityEngine.EventSystems;

public class TorreControleVenda : MonoBehaviour
{
    [Header("Economia")]
    public int valorVenda = 50; // Quanto o jogador recebe ao vender

    // Chamado automaticamente pelo Unity ao clicar com o mouse sobre o Collider2D da torre
    void OnMouseDown()
    {
        // 1. Bloqueia qualquer clique se o Pause ou as Opções estiverem abertos
        if (PauseManager.Instancia != null && PauseManager.Instancia.EstaPausado)
            return;

        // 2. Bloqueia o clique se o mouse estiver em cima de qualquer botão ou painel da UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Encontra o gerenciador do menu na tela e abre o painel para ESTA torre
        MenuTorreUI menuUI = FindFirstObjectByType<MenuTorreUI>();
        if (menuUI != null)
        {
            menuUI.AbrirMenu(this);
        }
    }
}