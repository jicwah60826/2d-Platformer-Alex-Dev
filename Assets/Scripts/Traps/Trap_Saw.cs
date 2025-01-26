using ES3Types;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float coolDown = 1f;

    [SerializeField]
    private Transform[] wayPoint;

    [SerializeField]
    private Vector3[] wayPointPosition;

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
        // get the vector3 data for all the waypoints before we do any moving
        GetWayPointsInfo();

        // snap saw to first waypoint poistion
        transform.position = wayPointPosition[0];
    }

    private void GetWayPointsInfo()
    {
        wayPointPosition = new Vector3[wayPoint.Length];

        // get all waypoint positions and store all to wayPointLocations array
        for (int i = 0; i < wayPoint.Length; i++)
        {
            wayPointPosition[i] = wayPoint[i].position;
        }
    }

    private void Update()
    {
        anim.SetBool("active", canMove);

        if (!canMove)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            wayPointPosition[waypointIndex],
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, wayPointPosition[waypointIndex]) < .1f)
        {
            if (waypointIndex == wayPointPosition.Length - 1 || waypointIndex == 0)
            {
                moveDirection = moveDirection * -1;

                StartCoroutine(StopMovementCo(coolDown));
            }

            waypointIndex = waypointIndex + moveDirection;
            // flip the saw position based on the next waypoin's position
            sr.flipX = transform.position.x < wayPointPosition[waypointIndex].x;
        }
    }

    private IEnumerator StopMovementCo(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
    }
}
