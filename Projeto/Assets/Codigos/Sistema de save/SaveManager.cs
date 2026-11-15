using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instancia { get; private set; }

    private const string CHAVE_INDICE_DESBLOQUEADO = "IndiceCenaDesbloqueado";

    // Índice 2 = Primeira fase jogável no Build Settings (0 = Boot, 1 = Menu)
    private const int INDICE_PRIMEIRA_FASE = 2;

    public int indiceMaximoDesbloqueado { get; private set; } = INDICE_PRIMEIRA_FASE;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            CarregarDados();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CarregarDados()
    {
        indiceMaximoDesbloqueado = PlayerPrefs.GetInt(CHAVE_INDICE_DESBLOQUEADO, INDICE_PRIMEIRA_FASE);
    }

    public void DesbloquearProximaFase(int indiceCenaConcluida)
    {
        int proximoIndice = indiceCenaConcluida + 1;

        if (proximoIndice > indiceMaximoDesbloqueado)
        {
            indiceMaximoDesbloqueado = proximoIndice;
            PlayerPrefs.SetInt(CHAVE_INDICE_DESBLOQUEADO, indiceMaximoDesbloqueado);
            PlayerPrefs.Save();
            Debug.Log($"[SAVE] Próxima fase (Build Index {indiceMaximoDesbloqueado}) desbloqueada!");
        }
    }

    public bool CenaEstaDesbloqueada(int indiceCena)
    {
        return indiceCena <= indiceMaximoDesbloqueado;
    }

    public void ResetarProgresso()
    {
        PlayerPrefs.DeleteKey(CHAVE_INDICE_DESBLOQUEADO);
        indiceMaximoDesbloqueado = INDICE_PRIMEIRA_FASE;
        PlayerPrefs.Save();
    }
}