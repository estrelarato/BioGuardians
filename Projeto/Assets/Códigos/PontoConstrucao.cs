using UnityEngine;

public class PontoConstrucao : MonoBehaviour
{
    public GameObject torreConstruida;

    void OnMouseDown()
    {
        ConstruirTorre();
    }

    void ConstruirTorre()
    {
        if (torreConstruida != null)
            return;

        GameObject torreSelecionada = GerenciadorConstrucao.instancia.torreSelecionada;

        if (torreSelecionada == null)
            return;

        Torre torre = torreSelecionada.GetComponent<Torre>();

        if (!GerenciadorDinheiro.instancia.TemDinheiro(torre.custo))
        {
            Debug.Log("Sem dinheiro!");
            return;
        }

        GerenciadorDinheiro.instancia.Gastar(torre.custo);

        torreConstruida = Instantiate(torreSelecionada, transform.position, Quaternion.identity);
    }
}