using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpForce;

    private Animator anim;

    [Header("Collision Info")]
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private float groundCheckDistance;

    private bool isGrounded;
    private float xInput;
    private bool facingRight = true;
    private int facingDir = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleCollision();
        HandleInput();
        HandleMovement();
        HandleFlip();
        HandleAnimations();
    }

    private void HandleInput()
    {
        // Left / Right input
        xInput = Input.GetAxisRaw("Horizontal");

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        // Move Player
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleFlip()
    {
        // Detect Direction for flip
        if (rb.velocity.x < 0 && facingRight || rb.velocity.x > 0 && !facingRight)
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
        Gizmos.DrawLine(
            transform.position,
            new Vector2(transform.position.x, transform.position.y - groundCheckDistance)
        );
    }
}
