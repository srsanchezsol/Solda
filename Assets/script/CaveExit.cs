using System.Collections;
using UnityEngine;

public class CaveExit : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPoint;
    public BoxCollider2D newCameraBounds;

    [Header("Fade Timing")]
    public float fadeOutWait = 0.35f;
    public float fadeInWait = 0.35f;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(TeleportRoutine(other.transform));
    }

    private IEnumerator TeleportRoutine(Transform currentPlayer)
    {
        isTeleporting = true;

        if (ScreenFader.instance != null)
            ScreenFader.instance.FadeOut();

        yield return new WaitForSeconds(fadeOutWait);

        if (currentPlayer != null && spawnPoint != null)
            currentPlayer.position = spawnPoint.position;

        CameraFollow cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
        if (cam != null)
        {
            cam.SetTarget(currentPlayer);

            if (newCameraBounds != null)
                cam.SetBounds(newCameraBounds);

            cam.SnapToTarget();
        }

        yield return new WaitForSeconds(0.05f);

        if (ScreenFader.instance != null)
            ScreenFader.instance.FadeIn();

        yield return new WaitForSeconds(fadeInWait);

        isTeleporting = false;
    }
}