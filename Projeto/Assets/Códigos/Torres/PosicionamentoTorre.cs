using UnityEngine;

public class PosicionamentoTorre : MonoBehaviour
{
    public Transform localAtual;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LocalDeConstrucao"))
        {
            localAtual = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LocalDeConstrucao") && localAtual == other.transform)
        {
            localAtual = null;
        }
    }
}