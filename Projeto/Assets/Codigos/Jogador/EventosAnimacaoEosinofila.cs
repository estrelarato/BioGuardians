using UnityEngine;

public class EventosAnimacaoEosinofila : MonoBehaviour
{
    private TropaEosinofila tropaPai;

    void Awake()
    {
        tropaPai = GetComponentInParent<TropaEosinofila>();
    }

    public void DispararArma1()
    {
        if (tropaPai != null) tropaPai.DispararArma1();
    }

    public void DispararArma2()
    {
        if (tropaPai != null) tropaPai.DispararArma2();
    }
}