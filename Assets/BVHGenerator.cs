using UnityEngine;
using System.Collections.Generic;

public class BVHGenerator : MonoBehaviour
{
    private class BVHNode
    {
        public Bounds bounds;
        public BVHNode left;
        public BVHNode right;
        public MeshCollider collider;
    }

    private BVHNode root;
    private bool bvhAtivo = true;

    void Start()
    {
        GenerateHitboxes();
        BuildBVH();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            bvhAtivo = !bvhAtivo;
        }
    }

    void GenerateHitboxes()
    {
        MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>();
        
        foreach (MeshFilter meshFilter in meshes)
        {
            GameObject part = meshFilter.gameObject;
            MeshCollider collider = part.GetComponent<MeshCollider>();
            if (collider == null)
            {
                collider = part.AddComponent<MeshCollider>();
            }
            collider.convex = true; // Necessário para colisão física
        }
    }

    void BuildBVH()
    {
        if (!bvhAtivo) return;
        
        List<MeshCollider> colliders = new List<MeshCollider>(GetComponentsInChildren<MeshCollider>());
        root = BuildBVHRecursive(colliders);
    }

    private BVHNode BuildBVHRecursive(List<MeshCollider> colliders)
    {
        if (colliders.Count == 0) return null;
        
        BVHNode node = new BVHNode();
        if (colliders.Count == 1)
        {
            node.collider = colliders[0];
            node.bounds = colliders[0].bounds;
            return node;
        }

        Bounds totalBounds = colliders[0].bounds;
        foreach (var collider in colliders)
        {
            totalBounds.Encapsulate(collider.bounds);
        }
        node.bounds = totalBounds;

        colliders.Sort((a, b) => a.bounds.center.x.CompareTo(b.bounds.center.x));
        int mid = colliders.Count / 2;

        node.left = BuildBVHRecursive(colliders.GetRange(0, mid));
        node.right = BuildBVHRecursive(colliders.GetRange(mid, colliders.Count - mid));

        return node;
    }

    public bool RaycastBVH(Vector3 origin, Vector3 direction, out RaycastHit hit)
{
    return RaycastBVHRecursive(root, origin, direction, out hit);
}

private bool RaycastBVHRecursive(BVHNode node, Vector3 origin, Vector3 direction, out RaycastHit hit)
{
    hit = new RaycastHit();

    if (node == null)
        return false;

    // Testa primeiro o bounding volume da raiz
    if (!node.bounds.IntersectRay(new Ray(origin, direction)))
        return false;

    // Se for um nó folha, testamos a malha real
    if (node.collider != null)
    {
        return node.collider.Raycast(new Ray(origin, direction), out hit, Mathf.Infinity);
    }

    // Testamos os filhos recursivamente
    RaycastHit leftHit, rightHit;
    bool hitLeft = RaycastBVHRecursive(node.left, origin, direction, out leftHit);
    bool hitRight = RaycastBVHRecursive(node.right, origin, direction, out rightHit);

    // Retorna o hit mais próximo
    if (hitLeft && hitRight)
    {
        hit = (leftHit.distance < rightHit.distance) ? leftHit : rightHit;
        return true;
    }
    return hitLeft || hitRight;
}

void OnDrawGizmos()
{
    if (root != null)
    {
        DrawBVH(root);
    }
}

void DrawBVH(BVHNode node)
{
    if (node == null) return;

    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(node.bounds.center, node.bounds.size);

    DrawBVH(node.left);
    DrawBVH(node.right);
}


}
