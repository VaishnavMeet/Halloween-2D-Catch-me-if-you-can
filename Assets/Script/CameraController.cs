using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;           // Reference to the player
    public float smoothSpeed = 50f;     // Speed at which the camera follows
    public float minX=0;                 // Left boundary for the camera
    public float maxX=5;                 // Right boundary for the camera
    public float cameraStep = 4f;      // Distance to move camera when player is out of view

    private Camera cam;

    void Start()
    {
        player= GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Check if player is visible on screen
        Vector3 viewportPos = cam.WorldToViewportPoint(player.position);

        if (viewportPos.x < 0f)
        {
            // Player is off-screen to the left
            float targetX = Mathf.Clamp(transform.position.x - cameraStep, minX, maxX);
            Vector3 newPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
        }
        else if (viewportPos.x > 1f)
        {
            // Player is off-screen to the right
            float targetX = Mathf.Clamp(transform.position.x + cameraStep, minX, maxX);
            Vector3 newPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
