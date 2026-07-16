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


        MoverTorre();


        // BOTÃO DIREITO = CANCELAR
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelarConstrucao();
            return;
        }


        // BOTÃO ESQUERDO = CONSTRUIR
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TentarConstruir();
        }
    }



    void MoverTorre()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mouse.z = 0;

        objetoAtual.transform.position = mouse;
    }



    void TentarConstruir()
    {
        Vector3 posicao = objetoAtual.transform.position;


        Collider2D local = Physics2D.OverlapPoint(posicao);


        if (local != null && local.CompareTag("LocalDeConstrucao"))
        {
            objetoAtual.transform.position = local.transform.position;


            Torre torre = objetoAtual.GetComponent<Torre>();

            if (torre != null)
            {
                torre.FinalizarConstrucao();
            }


            objetoAtual = null;
        }
    }



    void CancelarConstrucao()
    {
        if (objetoAtual != null)
        {
            Destroy(objetoAtual);

            objetoAtual = null;
        }
    }



    public void CriarObjeto()
    {
        if (objetoAtual != null)
            return;


        objetoAtual = Instantiate(prefab);


        Torre torre = objetoAtual.GetComponent<Torre>();

        if (torre != null)
        {
            // garante que nasce desativada
            torre.emConstrucao = true;
        }
    }
}