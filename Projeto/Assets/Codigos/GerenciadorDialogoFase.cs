using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[System.Serializable]
public class FalaFase
{
    public string nomePersonagem;
    public Sprite retratoPersonagem;
    [TextArea(2, 4)]
    public string texto;
}

public class GerenciadorDialogoFase : MonoBehaviour
{
    public static bool DialogoEmAndamento { get; private set; }

    [Header("Painel de Diálogo na UI da Fase")]
    public GameObject painelDialogo;
    public Image imagemRetrato;
    public TextMeshProUGUI textoNome;
    public TextMeshProUGUI textoFala;

    [Header("Configurações")]
    public float velocidadeDigitacao = 0.025f;
    public AudioClip somFala;

    [Header("Falas do Início da Fase")]
    public List<FalaFase> falasIniciais = new List<FalaFase>();

    private int indiceAtual = 0;
    private bool estaDigitando = false;
    private Coroutine corrotinaDigitacao;

    void Start()
    {
        if (falasIniciais.Count > 0)
        {
            IniciarDialogo();
        }
        else
        {
            DialogoEmAndamento = false;
            if (painelDialogo != null) painelDialogo.SetActive(false);
        }
    }

    void Update()
    {
        if (!DialogoEmAndamento) return;

        bool clicou = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                      (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame));

        if (clicou)
        {
            AvancarDialogo();
        }
    }

    public void IniciarDialogo()
    {
        DialogoEmAndamento = true;
        indiceAtual = 0;

        if (painelDialogo != null) painelDialogo.SetActive(true);
        Time.timeScale = 0f;

        ExibirFala(0);
    }

    public void AvancarDialogo()
    {
        if (estaDigitando)
        {
            if (corrotinaDigitacao != null) StopCoroutine(corrotinaDigitacao);
            textoFala.text = falasIniciais[indiceAtual].texto;
            estaDigitando = false;
            return;
        }

        if (AudioManager.Instancia != null && somFala != null)
        {
            AudioManager.Instancia.TocarSomUI(somFala);
        }

        indiceAtual++;
        if (indiceAtual < falasIniciais.Count)
        {
            ExibirFala(indiceAtual);
        }
        else
        {
            EncerrarDialogo();
        }
    }

    void ExibirFala(int indice)
    {
        FalaFase fala = falasIniciais[indice];

        if (textoNome != null) textoNome.text = fala.nomePersonagem;

        if (imagemRetrato != null)
        {
            imagemRetrato.gameObject.SetActive(fala.retratoPersonagem != null);
            imagemRetrato.sprite = fala.retratoPersonagem;
        }

        if (corrotinaDigitacao != null) StopCoroutine(corrotinaDigitacao);
        corrotinaDigitacao = StartCoroutine(DigitarTextoRealtime(fala.texto));
    }

    private IEnumerator DigitarTextoRealtime(string frase)
    {
        estaDigitando = true;
        textoFala.text = "";

        foreach (char letra in frase)
        {
            textoFala.text += letra;
            yield return new WaitForSecondsRealtime(velocidadeDigitacao);
        }

        estaDigitando = false;
    }

    public void EncerrarDialogo()
    {
        DialogoEmAndamento = false;
        if (painelDialogo != null) painelDialogo.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        DialogoEmAndamento = false;
    }
}