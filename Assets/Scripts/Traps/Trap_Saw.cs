using ES3Types;
using System.Collections;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float coolDown = 1f;
    [SerializeField] private Transform[] wayPoints;
    private SpriteRenderer sr;
    private Animator anim;


    public int waypointIndex = 1;
    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator> ();
        sr = GetComponent<SpriteRenderer> ();
    }

    private void Start()
    {
        // start at waypoint index 0
        transform.position = wayPoints[0].position;
    }

    private void Update()
    {
        anim.SetBool("active", canMove);

        if (!canMove) return;

            transform.position = Vector2.MoveTowards(transform.position, wayPoints[waypointIndex].position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, wayPoints[waypointIndex].position) < .1f)
            {
            //move to next waypoint
            waypointIndex++;
            // flip the sprite each time we iterate
            sr.flipX = !sr.flipX;

                if (waypointIndex >= wayPoints.Length)
                {
                    //reset to starting waypoint
                    waypointIndex = 0;
                    StartCoroutine(StopMovementCo(coolDown));
                }

            }
    }

    private IEnumerator StopMovementCo(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
    }

}
