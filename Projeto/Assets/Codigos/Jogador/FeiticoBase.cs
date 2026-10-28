using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class FeiticoBase : MonoBehaviour
{
    [Header("Configurações do Feitiço")]
    public string nomeFeitico = "Antibiótico";
    public float tempoCooldown = 20f;
    public GameObject prefabEfeitoArea; // Prefab que será instanciado no chão
    public bool ehCuraDireta = false;
    public int quantidadeCuraBase = 5;

    [Header("UI do Cooldown")]
    public Image imagemOverlayCooldown; // Imagem semi-transparente do botão (Image Type: Filled)
    public TextMeshProUGUI textoCooldown;
    public Button botaoFeitico;

    private float tempoRestanteCooldown = 0f;
    private bool emCooldown = false;
    private bool mirandoNaTela = false;

    void Start()
    {
        if (imagemOverlayCooldown != null)
            imagemOverlayCooldown.fillAmount = 0f;

        if (textoCooldown != null)
            textoCooldown.text = "";
    }

    void Update()
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

        // Se o jogador clicou no feitiço e está escolhendo onde aplicar no mapa
        if (mirandoNaTela)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ExecutarFeiticoNoPonto();
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelarMira();
            }
        }
    }

    /// <summary>
    /// Chamado pelo OnClick() do Botão na UI
    /// </summary>
    public void TentarUsarFeitico()
    {
        if (emCooldown) return;

        if (ehCuraDireta)
        {
            AplicarCuraNaBase();
        }
        else
        {
            // Ativa o modo de mira para o jogador clicar no mapa
            mirandoNaTela = true;
            Debug.Log($"[FEITIÇO] {nomeFeitico} selecionado! Clique no mapa para lançar.");
        }
    }

    void ExecutarFeiticoNoPonto()
    {
        Vector3 posicaoMundo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        posicaoMundo.z = 0;

        if (prefabEfeitoArea != null)
        {
            Instantiate(prefabEfeitoArea, posicaoMundo, Quaternion.identity);
        }

        mirandoNaTela = false;
        IniciarCooldown();
    }

    void AplicarCuraNaBase()
    {
        VidaDoJogador sistemaVida = FindFirstObjectByType<VidaDoJogador>();
        if (sistemaVida != null)
        {
            // Restaura vida da base (garantindo que não ultrapasse o máximo)
            int novaVida = Mathf.Min(sistemaVida.GetVidaAtual() + quantidadeCuraBase, sistemaVida.GetVidaMaxima());
            
            // Para aplicar a cura, usamos o ajuste direto ou estendemos a VidaDoJogador
            int vidaParaAdicionar = novaVida - sistemaVida.GetVidaAtual();
            if (vidaParaAdicionar > 0)
            {
                // Como VidaDoJogador tem TomarDano, se passarmos valor negativo ele recupera vida visualmente
                sistemaVida.TomarDano(-vidaParaAdicionar);
            }

            Debug.Log($"[FEITIÇO] Cura aplicada na base! +{quantidadeCuraBase} HP");
            IniciarCooldown();
        }
    }

    void CancelarMira()
    {
        mirandoNaTela = false;
        Debug.Log("[FEITIÇO] Mira cancelada.");
    }

    void IniciarCooldown()
    {
        emCooldown = true;
        tempoRestanteCooldown = tempoCooldown;

        if (botaoFeitico != null)
            botaoFeitico.interactable = false;
    }

    void FinalizarCooldown()
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