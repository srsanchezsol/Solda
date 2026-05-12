using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    [Header("Attack")]
    public bool hasSword = false;
    public GameObject swordHitbox;
    public float attackDuration = 0.2f;
    public float attackCooldown = 0.14f;

    [Header("Attack Sync")]
    public bool useAnimationEvents = false;
    public float swordShowDelay = 0.15f;
    public float swordVisibleTime = 0.12f;

    [Header("Attack Position")]
    public float hitboxDistanceHorizontal = 1.0f;
    public float hitboxDistanceUp = 1.05f;
    public float hitboxDistanceDown = 0.82f;

    [Header("Attack Visuals")]
    public SpriteRenderer swordRightVisual;
    public SpriteRenderer swordLeftVisual;
    public SpriteRenderer swordUpVisual;
    public SpriteRenderer swordDownVisual;

    [Header("Character Visuals")]
    public SpriteRenderer bodyRenderer;

    [Header("Optional Extra Visuals")]
    public SpriteRenderer handRenderer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swordSfx;

    [Header("Mobile")]
    public Joystick joystick;

    [Header("Interaction")]
    public NPCInteraction currentNPC;

    [Header("Input Tuning")]
    public float inputDeadZone = 0.2f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private PlayerAnimator playerAnimator;
    private PlayerHealth playerHealth;

    private bool isAttacking = false;
    private bool canAttack = true;
    private bool mobileAttackPressed = false;

    private Vector2 lastAttackDirection = Vector2.down;
    private Coroutine swordWindowRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerHealth = GetComponent<PlayerHealth>();

        if (bodyRenderer == null)
            bodyRenderer = GetComponent<SpriteRenderer>();

        if (swordHitbox != null)
            swordHitbox.SetActive(false);

        HideAllAttackSwords();
        ResetVerticalFlipVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            HandleActionButton();

        if (ConsumeMobileAttack())
            HandleActionButton();

        if (Time.timeScale == 0f)
        {
            movement = Vector2.zero;

            if (playerAnimator != null)
                playerAnimator.UpdateAnimation(Vector2.zero);

            return;
        }

        if (!isAttacking)
        {
            float keyboardX = Input.GetAxisRaw("Horizontal");
            float keyboardY = Input.GetAxisRaw("Vertical");

            float joystickX = 0f;
            float joystickY = 0f;

            if (joystick != null)
            {
                joystickX = joystick.Horizontal;
                joystickY = joystick.Vertical;
            }

            Vector2 rawInput = new Vector2(keyboardX + joystickX, keyboardY + joystickY);
            rawInput.x = Mathf.Clamp(rawInput.x, -1f, 1f);
            rawInput.y = Mathf.Clamp(rawInput.y, -1f, 1f);

            if (rawInput.magnitude < inputDeadZone)
            {
                movement = Vector2.zero;
            }
            else
            {
                movement = GetFourDirection(rawInput);
                lastAttackDirection = movement;
            }

            ApplyFacingVisuals(movement);

            if (playerAnimator != null)
                playerAnimator.UpdateAnimation(movement);
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (Time.timeScale == 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (playerHealth != null && playerHealth.IsKnockedBack())
            return;

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * speed;
    }

    Vector2 GetFourDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return input.x > 0f ? Vector2.right : Vector2.left;
        else
            return input.y > 0f ? Vector2.up : Vector2.down;
    }

    void HandleActionButton()
    {
        if (isAttacking || !canAttack)
            return;

        if (currentNPC != null && currentNPC.IsDialogueOpen())
        {
            currentNPC.Interact();
            return;
        }

        if (SwordPickup.currentPickup != null)
        {
            SwordPickup.currentPickup.TryPickup();
            return;
        }

        if (currentNPC != null)
        {
            currentNPC.Interact();
            return;
        }

        if (Time.timeScale != 0f && hasSword)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        if (!hasSword)
            yield break;

        canAttack = false;
        isAttacking = true;
        movement = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Vector2 attackDirection = lastAttackDirection;
        if (attackDirection == Vector2.zero)
            attackDirection = Vector2.down;

        lastAttackDirection = attackDirection;

        ApplyFacingVisuals(attackDirection);

        if (playerAnimator != null)
        {
            playerAnimator.ForceDirection(attackDirection);
            playerAnimator.SetAttacking(true);
        }

        if (audioSource != null && swordSfx != null)
            audioSource.PlayOneShot(swordSfx);

        PositionSwordHitbox(attackDirection);
        ConfigureSwordHitboxShape(attackDirection);

        SwordHitBox sword = swordHitbox != null ? swordHitbox.GetComponent<SwordHitBox>() : null;
        if (sword != null)
            sword.SetAttackDirection(attackDirection);

        HideSwordNow();

        if (useAnimationEvents)
        {
            yield return new WaitForSeconds(attackDuration);
            HideSwordNow();
        }
        else
        {
            if (swordWindowRoutine != null)
                StopCoroutine(swordWindowRoutine);

            swordWindowRoutine = StartCoroutine(SwordWindowRoutine(attackDirection));
            yield return new WaitForSeconds(attackDuration);
            HideSwordNow();
        }

        if (playerAnimator != null)
            playerAnimator.SetAttacking(false);

        isAttacking = false;

        ApplyFacingVisuals(lastAttackDirection);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator SwordWindowRoutine(Vector2 attackDirection)
    {
        if (swordShowDelay > 0f)
            yield return new WaitForSeconds(swordShowDelay);

        ShowSwordNow(attackDirection);

        if (swordVisibleTime > 0f)
            yield return new WaitForSeconds(swordVisibleTime);

        HideSwordNow();
        swordWindowRoutine = null;
    }

    void ApplyFacingVisuals(Vector2 direction)
    {
        if (direction.x > 0.1f)
        {
            SetFlipX(false);
        }
        else if (direction.x < -0.1f)
        {
            SetFlipX(true);
        }
        else if (Mathf.Abs(direction.y) > 0.1f)
        {
            ResetVerticalFlipVisuals();
        }
    }

    void SetFlipX(bool flipped)
    {
        if (bodyRenderer != null)
            bodyRenderer.flipX = flipped;

        if (handRenderer != null)
            handRenderer.flipX = flipped;
    }

    void ResetVerticalFlipVisuals()
    {
        SetFlipX(false);
    }

    void ShowAttackSword(Vector2 attackDirection)
    {
        HideAllAttackSwords();

        if (attackDirection == Vector2.right && swordRightVisual != null)
            swordRightVisual.enabled = true;
        else if (attackDirection == Vector2.left && swordLeftVisual != null)
            swordLeftVisual.enabled = true;
        else if (attackDirection == Vector2.up && swordUpVisual != null)
            swordUpVisual.enabled = true;
        else if (attackDirection == Vector2.down && swordDownVisual != null)
            swordDownVisual.enabled = true;
    }

    void HideAllAttackSwords()
    {
        if (swordRightVisual != null) swordRightVisual.enabled = false;
        if (swordLeftVisual != null) swordLeftVisual.enabled = false;
        if (swordUpVisual != null) swordUpVisual.enabled = false;
        if (swordDownVisual != null) swordDownVisual.enabled = false;
    }

    void ShowSwordNow(Vector2 attackDirection)
    {
        ShowAttackSword(attackDirection);

        if (swordHitbox != null)
            swordHitbox.SetActive(true);
    }

    void HideSwordNow()
    {
        HideAllAttackSwords();

        if (swordHitbox != null)
            swordHitbox.SetActive(false);
    }

    void PositionSwordHitbox(Vector2 attackDirection)
    {
        if (swordHitbox == null)
            return;

        Vector3 offset = Vector3.zero;

        if (attackDirection == Vector2.right)
            offset = new Vector3(hitboxDistanceHorizontal, 0f, 0f);
        else if (attackDirection == Vector2.left)
            offset = new Vector3(-hitboxDistanceHorizontal, 0f, 0f);
        else if (attackDirection == Vector2.up)
            offset = new Vector3(0f, hitboxDistanceUp, 0f);
        else if (attackDirection == Vector2.down)
            offset = new Vector3(0f, -hitboxDistanceDown, 0f);

        swordHitbox.transform.localPosition = offset;
    }

    void ConfigureSwordHitboxShape(Vector2 attackDirection)
    {
        if (swordHitbox == null)
            return;

        BoxCollider2D box = swordHitbox.GetComponent<BoxCollider2D>();
        if (box == null)
            return;

        if (attackDirection == Vector2.left || attackDirection == Vector2.right)
        {
            box.size = new Vector2(1.35f, 0.55f);
            box.offset = Vector2.zero;
        }
        else if (attackDirection == Vector2.up)
        {
            box.size = new Vector2(1.05f, 1.3f);
            box.offset = new Vector2(0f, 0.05f);
        }
        else if (attackDirection == Vector2.down)
        {
            box.size = new Vector2(1.05f, 1.15f);
            box.offset = new Vector2(0f, -0.03f);
        }
    }

    public void ShowSwordEvent()
    {
        ShowSwordNow(lastAttackDirection);
    }

    public void HideSwordEvent()
    {
        HideSwordNow();
    }

    public void GetSword()
    {
        hasSword = true;
        Debug.Log("Espada obtenida");
    }

    public void MobileAttack()
    {
        mobileAttackPressed = true;
    }

    public bool ConsumeMobileAttack()
    {
        if (mobileAttackPressed)
        {
            mobileAttackPressed = false;
            return true;
        }

        return false;
    }

    public void SetCurrentNPC(NPCInteraction npc)
    {
        currentNPC = npc;
    }

    public void ClearCurrentNPC(NPCInteraction npc)
    {
        if (currentNPC == npc)
            currentNPC = null;
    }

    public void MoveUp() { }
    public void MoveDown() { }
    public void MoveLeft() { }
    public void MoveRight() { }
    public void StopMove() { }
}