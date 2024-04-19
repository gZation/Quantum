using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    private Animator anim;
    //private NetworkAnimatorClientAuth nanim;
    public PlayerMovement move;
    private PlayerCollision coll;
    [HideInInspector]
    public SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<PlayerCollision>();
        move = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();

        //nanim = GetComponent<NetworkAnimatorClientAuth>();
    }

    void Update()
    {
        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("onWall", coll.onWall);
        anim.SetBool("onWallRight", coll.onRightWall);
        if (move != null)
        {
            anim.SetBool("wallSlide", move.wallSlide);
            anim.SetBool("canMove", move.canMove);
            anim.SetBool("isDashing", move.isDashing);
        } else
        {
            SetMovementRef();
        }
    }

    public void SetHorizontalMovement(float x, float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", x);
        anim.SetFloat("VerticalAxis", y);
        anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        /*if (!GameManager.instance.IsNetworked()) { 
            anim.SetTrigger(trigger);
        } else
        {
            nanim.SetTrigger(trigger);
        }*/

        anim.SetTrigger(trigger);
    }

    public void Flip(int side)
    {
        if (move != null && move.wallSlide)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }

        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }

    public void SetMovementRef()
    {
        move = GetComponentInParent<PlayerMovement>();
    }
}