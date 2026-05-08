using UnityEngine;

public class GerenciadorDinheiro : MonoBehaviour
{
    public static GerenciadorDinheiro instancia;

    public int dinheiro = 100;

    void Awake()
    {
        instancia = this;
    }

    public bool TemDinheiro(int custo)
    {
        return dinheiro >= custo;
    }

    public void Gastar(int custo)
    {
        dinheiro -= custo;
    }

    public void Adicionar(int valor)
    {
        dinheiro += valor;
    }
}