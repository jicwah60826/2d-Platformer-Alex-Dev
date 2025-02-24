using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Local Vars
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    private float xInput;
    private float yInput;
    private bool facingRight = true;
    private int facingDir = 1;
    private bool canBeControlled = false;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float doubleJumpForce;
    private float defaultGravityScale;
    private bool canDoubleJump;

    [Header("Player Abilities")]
    [SerializeField]
    private bool doubleJumpAbility;

    [SerializeField]
    private bool wallJumpSlideAbility;

    [Header("Buffer Jump")]
    [SerializeField]
    private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;

    [Header("Coyote Jump")]
    [SerializeField]
    private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;

    [Header("Wall Interactions")]
    [SerializeField]
    private float wallJumpDuration = .6f;

    [SerializeField]
    private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Knockback")]
    [SerializeField]
    private float knockBackDuration = 0f;

    [SerializeField]
    private Vector2 knockBackPower;
    private bool isKnocked;

    [Header("Collision Info")]
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private float groundCheckDistance;

    [SerializeField]
    private float wallCheckDistance;

    [Header("Enemy Detection")]
    [SerializeField]
    private LayerMask whatIsEnemy;

    [SerializeField]
    private Transform enemyCheck;

    [SerializeField]
    private float enemyCheckRadius;

    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    [Header("VFX")]
    [SerializeField]
    private GameObject deathVfx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateAirborneStatus();

        if (!canBeControlled)
        {
            HandleAnimations();
            HandleCollision();
            return; // stop all below if we cannot be controlled
        }

        if (isKnocked)
            return;

        HandleEnemyDetection();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    private void HandleEnemyDetection()
    {
        //only do this while player is falling
        if (rb.linearVelocityY >= 0)
            return;

        //Create an array of colliders as local variable. Array will be all objects player is colliding with

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            enemyCheck.position,
            enemyCheckRadius,
            whatIsEnemy
        );

        foreach (var enemy in colliders)
        {
            Enemy newEmeny = enemy.GetComponent<Enemy>();

            if (newEmeny != null)
            {
                newEmeny.Die();
                Jump(); // bounce the player of enem head
            }
        }
    }

    public void RespawnFinished(bool finished)
    {
        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }
    }

    public void KnockBack(float sourceDamageXPos)
    {
        float knockBackDir = 1;

        if (transform.position.x < sourceDamageXPos)
        {
            //damage has come from the right - knock player to left.
            knockBackDir = -1;
        }

        if (isKnocked)
            return;

        StartCoroutine(KnockBackCo());
        rb.linearVelocity = new Vector2(knockBackPower.x * knockBackDir, knockBackPower.y);
    }

    public void Die()
    {
        Destroy(gameObject);
        Instantiate(deathVfx, transform.position, Quaternion.identity);
    }

    public void Push(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCoRoutine(direction, duration));
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

        if (rb.linearVelocity.y < 0)
        {
            ActivateCoyoteJump();
        }
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

    #region Buffer & Coyote Jump

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
            bufferJumpActivated = Time.time - 1; //reset
            Jump();
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;

    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;

    #endregion

    #region Jump

    private void JumpButton()
    {
        //local bool only within this function
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;

        if (isGrounded || coyoteJumpAvailable)
        {
            Jump();
            Debug.Log("Jump zz1");
        }
        else if (isWallDetected && !isGrounded && wallJumpSlideAbility)
        {
            WallJump();
            Debug.Log("Jump zz2");
        }
        else if (canDoubleJump && doubleJumpAbility)
        {
            DoubleJump();
            Debug.Log("Jump zz3");
        }

        //cancel at the end of each jump
        CancelCoyoteJump();
    }

    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    private void DoubleJump()
    {
        isWallJumping = false;
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    #endregion

    #region Wall Slide & Wall Jump

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

    private void HandleWallSlide()
    {
        if (!wallJumpSlideAbility)
            return;

        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = yInput < 0 ? 1 : .05f;

        if (canWallSlide == false)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);
    }

    #endregion

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
        anim.SetBool("wallJumpSlideAbility", wallJumpSlideAbility);
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
        Gizmos.color = Color.yellow;
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

        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
    }

    #region CoRoutines

    private IEnumerator KnockBackCo()
    {
        isKnocked = true;
        anim.SetBool("isKnocked", isKnocked);

        yield return new WaitForSeconds(knockBackDuration);

        isKnocked = false;

        anim.SetBool("isKnocked", isKnocked);
    }

    private IEnumerator PushCoRoutine(Vector2 direction, float duration)
    {
        // disable player controls
        canBeControlled = false;

        // remove all player velocity
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;
    }

    #endregion
}
