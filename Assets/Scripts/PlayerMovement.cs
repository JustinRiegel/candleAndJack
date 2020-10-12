using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    public float distanceFromCandle;
    // public Vector2 position;
    public Rigidbody2D candle;
    public PlayerStatus playerStatus;

    // Update is called once per frame
    // bad place for physics
    void Update()
    {
        // handle input here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

    }

    void FixedUpdate()
    {
        // default is 50 times per sec 
        // handle physics here 
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (Vector2.Distance(rb.position, candle.position) > 2)  //this does a lot of DPS, and it should probably be not in FixedUpdate
        {
            if (playerStatus.currentHealth > 0)
            {
                playerStatus.TakeDamage(1);
            }                       
        }

        if (Vector2.Distance(rb.position, candle.position) <= 2)  //this should probably be with the fuction above, but not in FixedUpdate. also these might want to be their own functions?
        {
            if (playerStatus.currentHealth < playerStatus.maxHealth)
            {
                playerStatus.TakeDamage(-1); 
            }

        }

    }
}
