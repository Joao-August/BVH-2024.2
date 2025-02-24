using UnityEngine;

public class Tiro : MonoBehaviour
{
    public Camera cameraJogador;
    public float alcance = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clique do mouse para atirar
        {
            Atirar();
        }
    }

    void Atirar()
    {
        Ray ray = cameraJogador.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Busca todos os BVH na cena
        BVH[] inimigos = FindObjectsOfType<BVH>();

        foreach (var inimigo in inimigos)
        {
            if (inimigo.VerificarInterseccao(ray))
            {
                Debug.Log("Acertou um inimigo!");
                return;
            }
        }

        // Caso contrário, faz um Raycast normal
        if (Physics.Raycast(ray, out hit, alcance))
        {
            Debug.Log("Acertou: " + hit.collider.name);
        }
    }
}
