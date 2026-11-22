using UnityEngine;
using UnityEngine.EventSystems;

public class TorreControleVenda : MonoBehaviour
{
    [Header("Economia")]
    public int valorVenda = 50; // Quanto o jogador recebe ao vender

    // Chamado automaticamente pelo Unity ao clicar com o mouse sobre o Collider2D da torre
    void OnMouseDown()
    {
        Debug.Log("[TESTE] 1. Clique detectado na torre: " + gameObject.name);

        // 1. Bloqueia qualquer clique se o Pause ou as Opções estiverem abertos
        if (PauseManager.Instancia != null && PauseManager.Instancia.EstaPausado)
        {
            Debug.LogWarning("[TESTE] Bloqueado pelo PauseManager (EstaPausado = true).");
            return;
        }

        // 2. Bloqueia o clique se o mouse estiver em cima de qualquer botão ou painel da UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.LogWarning("[TESTE] Bloqueado porque o mouse está em cima de algum elemento de UI (Raycast Target).");
            return;
        }

        // Encontra o gerenciador do menu na tela e abre o painel para ESTA torre
        MenuTorreUI menuUI = FindFirstObjectByType<MenuTorreUI>();
        if (menuUI != null)
        {
            Debug.Log("[TESTE] 2. MenuTorreUI encontrado! Chamando AbrirMenu...");
            menuUI.AbrirMenu(this);
        }
        else
        {
            Debug.LogError("[TESTE] ERRO: Nenhum MenuTorreUI ativo foi encontrado na cena!");
        }
    }
}