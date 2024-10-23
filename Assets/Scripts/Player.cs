using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D theRB;
    [SerializeField]
    private float moveSpeed;
 
    // Update is called once per frame
    void Update()
    {
 
        // Move Player
        theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, theRB.velocity.y); 
 
    }

}
