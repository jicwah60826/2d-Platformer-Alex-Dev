using TMPro.EditorUtilities;
using UnityEngine;

public class Enemy_Mushroom : Enemy
{
    private BoxCollider2D enemyCollider;

    protected override void Awake()
    {
        base.Awake();

        enemyCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        anim.SetFloat("xVelocity", rb.linearVelocity.x);

        if (isDead)
            return;

        HandleMovement();
        HandleCollision();

        if (!isGroundInFrontDetected || isWallDetected)
        {
            if (!isGrounded)
                return;

            Flip();
            //reset the idleTimer every time
            idleTimer = idleDuration;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0)
            return;

        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocityY);
    }

    public override void Die()
    {
        base.Die();

        enemyCollider.enabled = false;
    }
}
