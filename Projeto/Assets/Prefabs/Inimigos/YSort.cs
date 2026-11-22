using UnityEngine;
using UnityEngine.Rendering;

public class YSort : MonoBehaviour
{
    private static int globalCounter = 0;
    private SortingGroup sortingGroup;
    private SpriteRenderer[] spriteRenderers;
    private int idOffset;

    void Awake()
    {
        // Procura o SortingGroup no pai ou nos filhos
        sortingGroup = GetComponentInChildren<SortingGroup>();
        
        // Se não tiver SortingGroup, pega TODOS os SpriteRenderers dos filhos
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        idOffset = globalCounter % 50;
        globalCounter++;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        if (pos.z != 0f)
        {
            pos.z = 0f;
            transform.position = pos;
        }

        int sortOrder = Mathf.RoundToInt(pos.y * -100f) + idOffset;

        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = sortOrder;
        }
        else if (spriteRenderers != null)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null)
                    spriteRenderers[i].sortingOrder = sortOrder;
            }
        }
    }
}