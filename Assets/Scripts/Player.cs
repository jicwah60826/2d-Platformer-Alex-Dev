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
    private bool facingRight = true;
    private int facingDir = 1;

    private bool canDoubleJump;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float doubleJumpForce;

    [Header("Collision Info")]
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private float groundCheckDistance;

    [SerializeField]
    private float wallCheckDistance;

    [SerializeField]
    private float wallSlideSpeed;
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
        HandleCollision();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleAnimations();
    }

    private void HandleWallSlide()
    {
        if (isWallDetected && rb.velocity.y < 0)
        {
            WallSliding();
        }
    }

    private void WallSliding()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideSpeed);
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
    }

    private void HandleInput()
    {
        // Left / Right input
        xInput = Input.GetAxisRaw("Horizontal");

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        canDoubleJump = false;
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
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        if (isWallDetected)
        {
            return;
        }
        
        // Move Player
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleFlip()
    {
        // Detect Direction for flip (depends on input from the player - NOT the movement of the character)
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
        {
            Flip();
            facingRight = !facingRight;
        }
    }

    private void Flip()
    {
        // Flip character
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
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
