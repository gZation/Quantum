using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetPlayerMovement : NetworkBehaviour
{
    
    [SerializeField] private float speed = 10;
    

    Rigidbody2D rb;
    private Vector2 movementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //rb equals the rigidbody on the player
    }

    void Update()
    {
        if (!IsOwner) return;
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        
        if (Input.GetButtonDown("Jump")) {
            rb.velocity = new Vector2(rb.velocity.x, 5f);
        }
    }

    void FixedUpdate() 
    {
        rb.velocity = new Vector2(movementDirection.x * speed, rb.velocity.y);
    }
}