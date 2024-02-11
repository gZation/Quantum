using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrows : PlayerMovement
{
    protected override Vector2 GetMovementDirection()
    {
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

        return movementDirection.normalized;
    }

    protected override Vector2 GetRawInput()
    {
        float xRaw = Input.GetAxisRaw("HorizontalArrows");
        float yRaw = Input.GetAxisRaw("VerticalArrows");
        return new Vector2(xRaw, yRaw);
    }
    public override bool IsJump()
    {
        return Input.GetKeyDown(KeyCode.O);
    }

    public override bool IsDash()
    {
        return Input.GetKeyDown(KeyCode.P);
    }

    public override bool IsQLock()
    {
        return Input.GetKeyDown(KeyCode.I);
    }
}