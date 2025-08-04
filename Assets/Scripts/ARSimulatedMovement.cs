using UnityEngine;

public class ARSimulatedMovement : MonoBehaviour
{
    public float speed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down
        float moveY = 0f;

        if (Input.GetKey(KeyCode.E)) moveY = 1f;
        if (Input.GetKey(KeyCode.Q)) moveY = -1f;

        Vector3 move = new Vector3(moveX, moveY, moveZ) * speed * Time.deltaTime;
        transform.Translate(move);        
    }
}
