using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class BotaoVoltarOpcoes : MonoBehaviour
{
    [Header("Navegação")]
    [Tooltip("Índice da cena Menu no Build Settings (padrão = 1)")]
    public int buildIndexMenu = 1;

    void Awake()
    {
        // Se veio de uma fase (PauseManager ativo), destrói apenas o EventSystem da cena Opções
        if (PauseManager.Instancia != null)
        {
            EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            foreach (EventSystem es in eventSystems)
            {
                if (es.gameObject.scene == gameObject.scene)
                {
                    Destroy(es.gameObject);
                    break;
                }
            }
        }
    }

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Voltar);
    }

    public void Voltar()
    {
        // Se existe um PauseManager ativo na memória, fecha as Opções e reabre o painel de Pause
        if (PauseManager.Instancia != null)
        {
            PauseManager.Instancia.FecharOpcoes();
        }
        else
        {
            // Caso contrário, veio do Menu Principal e volta para ele
            Time.timeScale = 1f;
            SceneManager.LoadScene(buildIndexMenu);
        }
    }
}