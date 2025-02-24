using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float verticalSpeed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A e D
        float moveZ = Input.GetAxis("Vertical");   // W e S
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            moveY = verticalSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            moveY = -verticalSpeed * Time.deltaTime;
        }

        Vector3 move = new Vector3(moveX, moveY, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
