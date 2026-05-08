using UnityEngine;

public class BotaoTorre : MonoBehaviour
{
    public GameObject torrePrefab;

    public void Selecionar()
    {
        GerenciadorConstrucao.instancia.SelecionarTorre(torrePrefab);
    }
}