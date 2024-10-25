using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    private float xInput;
    private float yInput;
    private bool facingRight = true;
    private int facingDir = 1;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Buffer Jump")]
    [SerializeField]
    private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;

    [Header("Wall Interactions")]
    [SerializeField]
    private float wallJumpDuration = .6f;

    [SerializeField]
    private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Knockback")]
    [SerializeField]
    private float knockBackDuration = 1f;

    [SerializeField]
    private Vector2 knockBackPower;
    private bool isKnocked;
    private bool canBeKnocked;

    [Header("Collision Info")]
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private float groundCheckDistance;

    [SerializeField]
    private float wallCheckDistance;

    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateAirborneStatus();

        if (isKnocked)
            return;

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    private void KnockBack()
    {
        if (isKnocked)
            return;

        StartCoroutine(KnockBackCo());
        anim.SetTrigger("knockBack");
        rb.linearVelocity = new Vector2(knockBackPower.x * -facingDir, knockBackPower.y);
    }

    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
        {
            HandleLanding();
        }

        if (!isGrounded && !isAirborne)
        {
            BecomeAirborne();
        }
    }

    private void BecomeAirborne()
    {
        isAirborne = true;
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        // Player Input
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    private void RequestBufferJump()
    {
        if (isAirborne)
        {
            //store current time in game that request occurs
            bufferJumpActivated = Time.time;
        }
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            //we still have time to allow the buffer jump
            bufferJumpActivated = 0; //reset
            Jump();
        }
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
    }

    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    private void DoubleJump()
    {
        isWallJumping = false;
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();
        StopCoroutine(WallJumpRoutine());
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

    private IEnumerator KnockBackCo()
    {
        canBeKnocked = false;
        isKnocked = true;

        yield return new WaitForSeconds(knockBackDuration);

        canBeKnocked = true;
        isKnocked = false;
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = yInput < 0 ? 1 : .05f;

        if (canWallSlide == false)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);
    }

    private void HandleCollision()
    {
        // Ground Check
        isGrounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            whatIsGround
        );
        // Wall Check
        isWallDetected = Physics2D.Raycast(
            transform.position,
            Vector2.right * facingDir,
            wallCheckDistance,
            whatIsGround
        );
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        if (isWallDetected)
            return;

        if (isWallJumping)
            return;

        // Move Player
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleFlip()
    {
        // Detect Direction for flip (depends on input from the player - NOT the movement of the character)
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Draw Ground Check Line
        Gizmos.DrawLine(
            transform.position,
            new Vector2(transform.position.x, transform.position.y - groundCheckDistance)
        );

        // Draw Wall Check Line
        Gizmos.DrawLine(
            transform.position,
            new Vector2(
                transform.position.x + (wallCheckDistance * facingDir),
                transform.position.y
            )
        );
    }
}
