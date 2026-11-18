using UnityEngine;

public class EventosAnimacaoMacrofaga : MonoBehaviour
{
    private TorreMacrofaga torrePai;

    void Awake()
    {
        torrePai = GetComponentInParent<TorreMacrofaga>();
    }

    // Este método aparecerá na lista do Animation Event
    public void ExecutarImpactoChao()
    {
        if (torrePai != null)
        {
            torrePai.ExecutarImpactoChao();
        }
    }
}