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

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouse.z = Camera.main.nearClipPlane;
        Vector3 mundo = Camera.main.ScreenToWorldPoint(mouse);
        mundo.z = 0;

        objetoAtual.transform.position = mouse;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D local = Physics2D.OverlapPoint(mouse);

            if (local != null && local.CompareTag("LocalDeConstrucao"))
            {
                objetoAtual.transform.position = local.transform.position;
                objetoAtual = null;
            }
        }
    }

    public void CriarObjeto()
    {
        if (objetoAtual == null)
            objetoAtual = Instantiate(prefab);
    }
}