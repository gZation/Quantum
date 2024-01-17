using System.Collections;
using UnityEngine;

public class MovementWASD : PlayerMovement
{

    //Arrow to move, Right shift to dash, right ctrl to jump
    public override void Update()
    {
        //if falling
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        Vector2 movementDirection = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            movementDirection.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementDirection.y -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementDirection.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementDirection.x += 1;
        }


        Walk(movementDirection);

        //Check if gounded or on a wall
        int mask = LayerMask.GetMask("ground");
        grounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, mask);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, mask)
            || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, mask);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, mask);

        if (grounded && !dashing)
        {
            wallJumped = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                Jump(Vector2.up);
            }
            if (onWall && !grounded)
            {
                WallJump();
            }
        }
        else if (onWall && !grounded && !wallJumped)
        {
            WallSlide();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash(movementDirection.x, movementDirection.y);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuantumLock();
        }
    }

    //GIZMOs
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
    }
}