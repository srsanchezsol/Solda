using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    public GameObject swordHitbox;
    public float attackDuration = 0.12f;

    private PlayerState playerState;
    private bool isAttacking = false;
    private SpriteRenderer seekerSprite;

    void Awake()
    {
        playerState = GetComponent<PlayerState>();
        seekerSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerState != null && playerState.hasSword && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DoAttack());
        }
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;

        if (seekerSprite != null)
            seekerSprite.enabled = false;

        if (swordHitbox != null)
            swordHitbox.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        if (swordHitbox != null)
            swordHitbox.SetActive(false);

        if (seekerSprite != null)
            seekerSprite.enabled = true;

        isAttacking = false;
    }
}