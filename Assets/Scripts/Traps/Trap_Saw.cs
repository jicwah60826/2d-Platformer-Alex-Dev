using ES3Types;
using System.Collections;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float coolDown = 1f;

    [SerializeField]
    private Transform[] wayPoints;
    private SpriteRenderer sr;
    private Animator anim;

    public int waypointIndex = 1;
    public int moveDirection = 1; //1 is forward, -1 is reverse
    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // start at waypoint index 0
        transform.position = wayPoints[0].position;
    }

    private void Update()
    {
        anim.SetBool("active", canMove);

        if (!canMove)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            wayPoints[waypointIndex].position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, wayPoints[waypointIndex].position) < .1f)
        {
            if (waypointIndex == wayPoints.Length - 1 || waypointIndex == 0)
            {
                moveDirection = moveDirection * -1;
            }

            waypointIndex = waypointIndex + moveDirection;
            Debug.Log("waypointIndex: " + waypointIndex);
        }
    }

    private IEnumerator StopMovementCo(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
    }
}
