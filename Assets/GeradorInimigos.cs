using UnityEngine;

public class GeradorInimigos : MonoBehaviour
{
    public GameObject prefabInimigo;

    void Start()
    {
        for (int i = 0; i < 500; i++)
        {
            Vector3 posicao = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            Quaternion rotacao = Quaternion.Euler(-90, 0, 0);
            Instantiate(prefabInimigo, posicao, rotacao);
        }
    }
}
