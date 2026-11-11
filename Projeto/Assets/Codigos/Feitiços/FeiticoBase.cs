using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public abstract class FeiticoBase : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public string nomeFeitico = "Feitiço";
    public int custoATP = 25;
    public float tempoCooldown = 20f;

    [Header("UI do Cooldown")]
    public Image imagemOverlayCooldown;
    public TextMeshProUGUI textoCooldown;
    public Button botaoFeitico;

    protected float tempoRestanteCooldown = 0f;
    protected bool emCooldown = false;
    protected bool mirandoNaTela = false;

    protected virtual void Start()
    {
        if (imagemOverlayCooldown != null)
            imagemOverlayCooldown.fillAmount = 0f;

        if (textoCooldown != null)
            textoCooldown.text = "";
    }

    protected virtual void Update()
    {
        // Gerencia o tempo de recarga
        if (emCooldown)
        {
            tempoRestanteCooldown -= Time.deltaTime;
            
            if (imagemOverlayCooldown != null)
                imagemOverlayCooldown.fillAmount = tempoRestanteCooldown / tempoCooldown;

            if (textoCooldown != null)
                textoCooldown.text = Mathf.Ceil(tempoRestanteCooldown).ToString();

            if (tempoRestanteCooldown <= 0f)
            {
                FinalizarCooldown();
            }
        }

        // Se estiver em modo de mira no mapa
        if (mirandoNaTela)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ConfirmarLancamento();
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelarMira();
            }
        }
    }

    /// <summary>
    /// Chamado pelo OnClick() do Botão na UI
    /// </summary>
    public virtual void TentarUsarFeitico()
    {
        if (emCooldown || mirandoNaTela) return;

        // Tenta gastar o ATP via GameManager
        if (GameManager.Instance != null && !GameManager.Instance.TentarGastarDinheiro(custoATP))
        {
            Debug.LogWarning($"[FEITIÇO] ATP insuficiente para usar {nomeFeitico}! Custo: {custoATP}");
            return;
        }

        ExecutarLogicaInicial();
    }

    /// <summary>
    /// Define se o feitiço é lançado direto ou abre mira no mapa
    /// </summary>
    protected abstract void ExecutarLogicaInicial();

    /// <summary>
    /// Lógica executada ao clicar em um ponto do mapa (se usar mira)
    /// </summary>
    protected abstract void AplicarEfeitoNoPonto(Vector3 posicaoMundo);

    private void ConfirmarLancamento()
    {
        Vector3 posicaoMundo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        posicaoMundo.z = 0;

        AplicarEfeitoNoPonto(posicaoMundo);

        mirandoNaTela = false;
        IniciarCooldown();
    }

    protected void CancelarMira()
    {
        if (mirandoNaTela)
        {
            mirandoNaTela = false;
            // Reembolsa o valor do feitiço caso o jogador cancele
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GanharDinheiro(custoATP);
            }
            Debug.Log($"[FEITIÇO] {nomeFeitico}: Mira cancelada e ATP reembolsado.");
        }
    }

    protected void IniciarCooldown()
    {
        emCooldown = true;
        tempoRestanteCooldown = tempoCooldown;

        if (botaoFeitico != null)
            botaoFeitico.interactable = false;
    }

    protected void FinalizarCooldown()
    {
        emCooldown = false;
        tempoRestanteCooldown = 0f;

        if (imagemOverlayCooldown != null)
            imagemOverlayCooldown.fillAmount = 0f;

        if (textoCooldown != null)
            textoCooldown.text = "";

        if (botaoFeitico != null)
            botaoFeitico.interactable = true;
    }
}