using ES3Types;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Trap_Arrow : Trap_Trampoline
{
    [Header("Arrow Mechanics")]
    [SerializeField]
    private bool rotationRight;

    [SerializeField]
    private float rotationSpeed = 120f;

    [SerializeField]
    private float coolDown;

    [Space]
    [SerializeField]
    private float scaleUpSpeed = 5f;

    [SerializeField]
    private Vector3 targetScale;

    private int direction = -1;

    private void Start()
    {
        transform.localScale = new Vector3(.3f, .3f, .3f);
    }

    private void Update()
    {
        HandleRotation();
        HandleScaleUp();
    }

    private void HandleScaleUp()
    {
        if (transform.localScale.x < targetScale.x)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                scaleUpSpeed * Time.deltaTime
            );
        }
    }

    private void HandleRotation()
    {
        direction = rotationRight ? -1 : 1; // if rotationRight = true, then -1, else set to 1.
        transform.Rotate(0, 0, (rotationSpeed * direction) * Time.deltaTime);
    }

    private void DestroyMe()
    {
        GameObject arrowPrefab = GameManager.instance.arrowPrefab;

        GameManager.instance.CreateObject(arrowPrefab, transform, coolDown);

        Destroy(gameObject); // only called via animation event on last frame of Trap_Arrow_Hit animation
    }
}
