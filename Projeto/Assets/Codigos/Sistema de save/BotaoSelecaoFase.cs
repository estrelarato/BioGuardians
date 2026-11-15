using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class BotaoSelecaoFase : MonoBehaviour
{
    [Header("Índice da Cena no Build Settings")]
    [Tooltip("2 = Pulmão, 3 = Pele, 4 = Corrente Sanguínea, 5 = Cérebro")]
    public int buildIndexDaFase = 2;

    private Button botao;

    void Awake()
    {
        botao = GetComponent<Button>();
        botao.onClick.AddListener(CarregarFase);
    }

    // Roda toda vez que o Painel de Seleção de Fases for ativado
    void OnEnable()
    {
        if (SaveManager.Instancia != null)
        {
            botao.interactable = SaveManager.Instancia.CenaEstaDesbloqueada(buildIndexDaFase);
        }
        else
        {
            botao.interactable = (buildIndexDaFase == 2);
        }
    }

    void CarregarFase()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndexDaFase);
    }
}