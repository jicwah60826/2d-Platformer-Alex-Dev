using ES3Types;
using System.Collections;
using UnityEngine;

public class Trap_Fan : MonoBehaviour
{
    private AreaEffector2D effector;
    private BoxCollider2D boxCollider;
    private Rigidbody2D playerRb;
    private float playerStartingGravity;
    private Player player;
    private Animator anim;

    [SerializeField]
    private float effectorAngle;

    [SerializeField]
    private float effectorStrength;

    [SerializeField]
    private float playerGravityonEnter;

    [SerializeField]
    private float randomStartDelay;

    private float randomDelay;

    private bool active;

    private void Awake()
    {
        effector = GetComponent<AreaEffector2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        //Store reference to the player object
        player = FindFirstObjectByType<Player>();

        // Store reference to the RB on the player object
        playerRb = player.GetComponent<Rigidbody2D>();

        // store player gravity on enter
        playerStartingGravity = playerRb.gravityScale;
        Debug.Log("playerStartingGravity: " + playerStartingGravity);

        //set effector parameters
        effector.forceAngle = effectorAngle;
        effector.forceMagnitude = effectorStrength;

        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        randomDelay = Random.Range(0, randomStartDelay);
        Debug.Log(randomDelay);
        StartCoroutine(StartFansCo(randomDelay));
    }

    private void Update()
    {
        HandleAnimation();
    }

    private IEnumerator StartFansCo(float delay)
    {
        active = false;
        yield return new WaitForSeconds(delay);
        active = true;
        anim.SetBool("active", active);
    }

    private void HandleAnimation() => anim.SetBool("active", active);

    private void OnTriggerEnter2D(Collider2D other)
    {
        //set Player gravity to 0
        playerRb.gravityScale = playerGravityonEnter;
        // remove all player velocity
        //playerRb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // set player gravity back to initial
        playerRb.gravityScale = playerStartingGravity;
    }
}
