using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform thePlatform;

    public Transform[] movePoints;
    private int currentPoint;

    public float moveSpeed;

    private SpriteRenderer sr;

    private void Awake()
    {
        //put saw or platform at the first move point
        transform.position = movePoints[0].position;

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // snap saw to first waypoint poistion
        transform.position = movePoints[0].position;

        // get the position of the starting waypoint
        float startingPointPosition = transform.position.x;
        Debug.Log("startingPointPosition: " + startingPointPosition);

        // get the position of the FIRST waypoint
        float firstWayPointxPosition = movePoints[1].position.x;
        Debug.Log("firstWayPointxPosition: " + firstWayPointxPosition);

        if (firstWayPointxPosition > startingPointPosition)
        {
            // if the first waypoint is on the RIGHT - flip the sprite on the X
            sr.flipX = true;
        }
        else
        {
            // if the first waypoint is on the LEFT - keep sprite flip x disabled
            sr.flipX = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        thePlatform.position = Vector3.MoveTowards(
            thePlatform.position,
            movePoints[currentPoint].position,
            moveSpeed * Time.deltaTime
        );

        if (thePlatform.position == movePoints[currentPoint].position)
        {
            currentPoint++;

            if (currentPoint >= movePoints.Length)
            {
                currentPoint = 0;
            }
        }
    }
}
