using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpriteUpdater : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerCollision col; 
    PlayerAnimation anim;

    int side = 1;

    Vector3 lastPosition;

    float checkcd = 0.5f;
    float timePassed = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<PlayerCollision>();
        anim = GetComponentInChildren<PlayerAnimation>();
        lastPosition = this.transform.position;
        timePassed = 0;
    }

    void Update()
    {
        timePassed += Time.deltaTime;

        if (timePassed > checkcd)
        {
            float x = this.transform.position.x - lastPosition.x;

            if (x > 0)
            {
                side = 1;
                anim.Flip(side);
            }
            if (x < 0)
            {
                side = -1;
                anim.Flip(side);
            }

            lastPosition = this.transform.position;
            timePassed = 0;
        }

        if ((side == 1 && col.onRightWall) || side == -1 && !col.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        if (col.wallSide != side)
            anim.Flip(side * -1);
    }
}
