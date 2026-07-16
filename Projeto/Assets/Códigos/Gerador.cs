using UnityEngine;
using UnityEngine.InputSystem;

public class Gerador : MonoBehaviour
{
    public GameObject prefab;
    private GameObject objetoAtual;

    void Update()
    {
        if (objetoAtual == null)
            return;

        // Posição do mouse no mundo
        Vector3 mundo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mundo.z = 0;

        // Faz a torre seguir o mouse
        objetoAtual.transform.position = mundo;

        // Clique esquerdo
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D local = Physics2D.OverlapPoint(mundo);

            if (local != null && local.CompareTag("LocalDeConstrucao"))
            {
                // Encaixa a torre no local
                objetoAtual.transform.position = local.transform.position;

                // Ativa a torre
                Torre torre = objetoAtual.GetComponent<Torre>();

                if (torre != null)
                {
                    torre.emConstrucao = false;
                }

                // Permite construir outra
                objetoAtual = null;
            }
        }
    }

    public void CriarObjeto()
    {
        if (objetoAtual == null)
        {
            objetoAtual = Instantiate(prefab);

            Torre torre = objetoAtual.GetComponent<Torre>();

            if (torre != null)
            {
                torre.emConstrucao = true;
            }
        }
    }
}