using UnityEngine;

public class RoomEntrance : MonoBehaviour
{
    public Transform spawnPoint;
    public BoxCollider2D newCameraBounds;
    public Transform cameraTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = spawnPoint.position;
        }
        else
        {
            other.transform.position = spawnPoint.position;
        }

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            if (newCameraBounds != null)
                cam.bounds = newCameraBounds;

            Transform targetPoint = cameraTarget != null ? cameraTarget : spawnPoint;

            cam.transform.position = new Vector3(
                targetPoint.position.x,
                targetPoint.position.y,
                cam.transform.position.z
            );
        }
    }
}