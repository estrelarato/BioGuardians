using UnityEngine;

public class InimigoEncapsulado : Inimigo
{
    [Header("Mecânica Biofilme / Cápsula")]
    public float vidaCapsula = 50f;
    public float vidaCapsulaAtual;
    public GameObject artefatoCapsulaVisual;
    
    private bool capsulaDestruida = false;

    private void Awake()
    {
        vidaCapsulaAtual = vidaCapsula;
    }

    // Oculta o ReceberDano do pai usando 'new'
    public new void ReceberDano(float quantidade)
    {
        // Se ainda possui a cápsula, o dano reduz o escudo primeiro
        if (!capsulaDestruida)
        {
            vidaCapsulaAtual -= quantidade;

            if (vidaCapsulaAtual <= 0)
            {
                capsulaDestruida = true;
                
                if (artefatoCapsulaVisual != null)
                {
                    artefatoCapsulaVisual.SetActive(false);
                }

                Debug.Log("[ENCAPSULADO] Cápsula de biofilme rompida!");
            }
            return;
        }

        // Quando a cápsula for destruída, chama o método ReceberDano normal da classe pai
        base.ReceberDano(quantidade);
    }

    /// <summary>
    /// Retorna true quando a cápsula for destruída, permitindo desaceleração.
    /// </summary>
    public bool PodeReceberLentidao()
    {
        return capsulaDestruida;
    }
}