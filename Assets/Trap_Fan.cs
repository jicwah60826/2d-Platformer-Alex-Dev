using UnityEngine;

public class Trap_Fan : MonoBehaviour
{
    private AreaEffector2D effector;
    private BoxCollider2D boxCollider;
    private Rigidbody2D playerRb;
    private float playerStartingGravity;
    private Player player;

    [SerializeField]
    private float effectorAngle;

    [SerializeField]
    private float effectorStrength;

    [SerializeField]
    private float playerGravityonEnter;

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    { //set Player gravity to 0
        playerRb.gravityScale = playerGravityonEnter;
        Debug.Log("playerStartingGravity set to 0");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // set player gravity back to initial
        playerRb.gravityScale = playerStartingGravity;
        Debug.Log("playerStartingGravity: " + playerStartingGravity);
    }
}
