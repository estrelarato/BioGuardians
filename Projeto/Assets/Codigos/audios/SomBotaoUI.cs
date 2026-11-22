using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SomBotaoUI : MonoBehaviour, IPointerEnterHandler
{
    [Header("Clipes de Áudio")]
    public AudioClip somClique;
    [Tooltip("Opcional: som ao passar o mouse por cima do botão")]
    public AudioClip somHover;

    private Button botao;

    void Awake()
    {
        botao = GetComponent<Button>();
    }

    void Start()
    {
        if (botao != null)
        {
            botao.onClick.AddListener(TocarSomClique);
        }
    }

    public void TocarSomClique()
    {
        if (AudioManager.Instancia != null && somClique != null)
        {
            AudioManager.Instancia.TocarSomUI(somClique);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (botao != null && !botao.interactable) return;

        if (AudioManager.Instancia != null && somHover != null)
        {
            AudioManager.Instancia.TocarSomUI(somHover);
        }
    }
}