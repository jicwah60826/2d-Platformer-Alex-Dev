using System.Threading;
using UnityEngine;

public class Trap_Saw_Circular : MonoBehaviour
{
    [SerializeField]
    private Transform theSaw;

    [SerializeField]
    private Transform[] movePoints;
    private int currentPoint;

    [SerializeField]
    private float moveSpeed;

    private void Start()
    {
        foreach (var movePoint in movePoints)
        {
            transform.SetParent(null);
        }
    }

    private void Update()
    {
        theSaw.position = Vector3.MoveTowards(
            theSaw.position,
            movePoints[currentPoint].position,
            moveSpeed * Time.deltaTime
        );

        if (theSaw.position == movePoints[currentPoint].position)
        {
            currentPoint++;

            if (currentPoint >= movePoints.Length)
            {
                currentPoint = 0;
            }
        }
    }
}
