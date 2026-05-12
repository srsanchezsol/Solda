using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10f;
    public BoxCollider2D bounds;

    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;

        FindPlayerIfNeeded();
        SnapToTarget();
    }

    void LateUpdate()
    {
        FindPlayerIfNeeded();

        if (target == null) return;

        Vector3 desiredPosition = GetClampedPosition(target.position);
        desiredPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    void FindPlayerIfNeeded()
    {
        if (target != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            target = player.transform;
    }

    public void SetBounds(BoxCollider2D newBounds)
    {
        bounds = newBounds;
        SnapToTarget();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SnapToTarget();
    }

    public void SnapToTarget()
    {
        FindPlayerIfNeeded();

        if (target == null) return;

        Vector3 snappedPosition = GetClampedPosition(target.position);
        snappedPosition.z = transform.position.z;

        transform.position = snappedPosition;
    }

    public void ForceSnapToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
            SnapToTarget();
        }
    }

    private Vector3 GetClampedPosition(Vector3 targetPosition)
    {
        float clampedX = targetPosition.x;
        float clampedY = targetPosition.y;

        if (bounds != null)
        {
            Bounds b = bounds.bounds;

            float minX = b.min.x + camHalfWidth;
            float maxX = b.max.x - camHalfWidth;
            float minY = b.min.y + camHalfHeight;
            float maxY = b.max.y - camHalfHeight;

            if (minX > maxX)
                clampedX = b.center.x;
            else
                clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);

            if (minY > maxY)
                clampedY = b.center.y;
            else
                clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        return new Vector3(clampedX, clampedY, 0f);
    }
}