using ES3Types;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    // script will be used by enemies through inheritence
    protected Animator anim;
    protected Rigidbody2D rb;

    [SerializeField]
    protected float moveSpeed = 2f;

    [SerializeField]
    protected GameObject damageTrigger;

    [Space]
    [SerializeField]
    protected float idleDuration = 1.5f;

    protected float idleTimer;

    [Header("Basic Collision")]
    [SerializeField]
    protected float groundCheckDistance = 1.1f;

    [SerializeField]
    protected float wallCheckDistance = .7f;

    [SerializeField]
    LayerMask whatIsGround;

    [SerializeField]
    protected Transform groundCheck;

    [Header("Death Details")]
    [SerializeField]
    private float deathImpactSpeed = 5f;

    [SerializeField]
    protected float deathRotationSpeed = 150;

    protected bool isDead;

    private int deathRotationDirection = 1;

    protected bool isWallDetected;
    protected bool isGrounded;
    protected bool isGroundInFrontDetected;

    // facing left by default
    protected int facingDir = -1;

    protected bool facingRight = false;

    protected virtual void Awake()
    {
        // called in ALL objects that inherit this class

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        idleTimer -= Time.deltaTime;

        if (isDead)
        {
            HandleDeathRotation();
        }
    }

    public virtual void Die()
    {
        // Trigger enemy death animation
        anim.SetTrigger("hit");
        isDead = true;
        damageTrigger.SetActive(false);
        // Make enemy jump a little
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, deathImpactSpeed);

        // 50% chance that enemy will rotate one way or the other.
        if (Random.Range(0, 100) < 50)
        {
            deathRotationDirection = deathRotationDirection * -1;
        }
    }

    private void HandleDeathRotation()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }

    protected virtual void HandleFlip(float xValue)
    {
        // Detect Direction for flip (depends on input from the player - NOT the movement of the character)
        if (xValue < 0 && facingRight || xValue > 0 && !facingRight)
        {
            Flip();
        }
    }

    protected virtual void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // Draw Ground Check frontal Line
        Gizmos.DrawLine(
            groundCheck.position,
            new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance)
        );

        // Is Grounded Line
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

    protected virtual void HandleCollision()
    {
        // Ground Check in front of enemy
        isGroundInFrontDetected = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            whatIsGround
        );

        // Grounded Check
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
}
