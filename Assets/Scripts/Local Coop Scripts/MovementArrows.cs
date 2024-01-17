using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrows : PlayerMovement
{
    //Arrow to move,Keypad 0 to dash, right ctrl to jump, right shift to quantum lock
    public override void Update()
    {
        //if falling
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.RightControl))
        {
            Vector2 m = Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            rb.velocity += m;
            if (sharingMomentum)
            {
                GameManager.instance.SendMomentum(m, this.gameObject);
            }
        }

        Vector2 movementDirection = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movementDirection.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementDirection.y -= 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementDirection.x -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
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

        if (Input.GetKeyDown(KeyCode.RightControl))
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

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Dash(movementDirection.x, movementDirection.y);
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
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