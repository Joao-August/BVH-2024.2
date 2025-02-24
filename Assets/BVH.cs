using UnityEngine;
using System.Collections.Generic;

public class BVH : MonoBehaviour
{
    private Bounds boundingVolume;
    private MeshCollider[] meshColliders;

    void Start()
    {
        meshColliders = GetComponentsInChildren<MeshCollider>();
        if (meshColliders.Length > 0)
        {
            ConstruirBVH();
        }
    }

    void ConstruirBVH()
    {
        // Inicializa com o primeiro MeshCollider
        boundingVolume = meshColliders[0].bounds;

        foreach (var col in meshColliders)
        {
            boundingVolume.Encapsulate(col.bounds);
        }
    }

    public bool VerificarInterseccao(Ray ray)
    {
        // Primeiro, verifica a colisão com o bounding volume raiz
        if (!boundingVolume.IntersectRay(ray)) return false;

        // Se passou, verifica as partes internas
        foreach (var collider in meshColliders)
        {
            if (collider.Raycast(ray, out RaycastHit hit, 100f))
            {
                Debug.Log("Acertou " + hit.collider.name);
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boundingVolume.center, boundingVolume.size);
    }
}
