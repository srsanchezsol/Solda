using UnityEngine;

public class SecretCaveEntrance : MonoBehaviour
{
    [Header("Teleport")]
    public Transform spawnPoint;

    [Header("Camera")]
    public BoxCollider2D newCameraBounds;

    [Header("Respawn After Death")]
    public Transform newRespawnPoint;
    public BoxCollider2D newRespawnCameraBounds;

    [Header("Objects On Teleport")]
    public GameObject objectToActivate;
    public GameObject objectToDeactivate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = spawnPoint.position;
        }
        else
        {
            other.transform.position = spawnPoint.position;
        }

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        if (objectToDeactivate != null)
            objectToDeactivate.SetActive(false);

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            if (newRespawnPoint != null)
                health.respawnPoint = newRespawnPoint;

            if (newRespawnCameraBounds != null)
                health.respawnCameraBounds = newRespawnCameraBounds;
        }

        CameraFollow cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;

        if (cam != null)
        {
            if (newCameraBounds != null)
                cam.SetBounds(newCameraBounds);

            cam.SetTarget(other.transform);
            cam.ForceSnapToPlayer();
        }
    }
}