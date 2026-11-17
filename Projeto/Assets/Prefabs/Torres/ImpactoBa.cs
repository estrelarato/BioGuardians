using UnityEngine;

public class impactoBa : MonoBehaviour
{
    private TropaBasofila scriptPai;

    void Awake()
    {
        // Busca automaticamente o script TropaBasofila no objeto pai
        scriptPai = GetComponentInParent<TropaBasofila>();
    }

    // Função que aparece no dropdown do Animation Event
    public void EventoImpactoGolpe()
    {
        if (scriptPai != null)
        {
            scriptPai.AplicarImpactoGolpe();
        }
    }
}