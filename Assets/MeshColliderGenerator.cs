using UnityEngine;

public class MeshColliderGenerator : MonoBehaviour
{
    void Start()
    {
        // Encontra todas as partes do corpo com SkinnedMeshRenderer e adiciona MeshCollider
        SkinnedMeshRenderer[] partes = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var parte in partes)
        {
            if (!parte.gameObject.GetComponent<MeshCollider>())
            {
                MeshCollider collider = parte.gameObject.AddComponent<MeshCollider>();
                collider.convex = false; // Mantém a precisão da malha
            }
        }
    }
}
