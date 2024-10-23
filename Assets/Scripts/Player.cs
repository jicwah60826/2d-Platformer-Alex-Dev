using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float moveSpeed;

    private float xInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
    }

    private void HandleMovement()
    {
        // Move Player
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }
}
