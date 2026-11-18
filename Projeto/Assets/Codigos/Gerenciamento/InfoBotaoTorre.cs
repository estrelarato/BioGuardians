using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InfoBotaoTorre : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referências do Painel Único")]
    public GameObject painelInfo;
    public TextMeshProUGUI textoNome;
    public TextMeshProUGUI textoCusto;
    public TextMeshProUGUI textoDescricao;

    [Header("Dados desta Torre")]
    public string nomeTorre = "Macrófago";
    public int custoATP = 75;
    [TextArea(2, 4)]
    public string descricao = "Causa dano em área aos patógenos próximos.";

    // Dispara automaticamente quando o mouse entra no botão
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (painelInfo == null) return;

        textoNome.text = nomeTorre;
        textoCusto.text = "ATP: " + custoATP;
        textoDescricao.text = descricao;

        painelInfo.SetActive(true);
    }

    // Dispara automaticamente quando o mouse sai do botão
    public void OnPointerExit(PointerEventData eventData)
    {
        if (painelInfo != null)
            painelInfo.SetActive(false);
    }

    // Garante que o painel feche se o botão for desativado
    private void OnDisable()
    {
        if (painelInfo != null)
            painelInfo.SetActive(false);
    }
}