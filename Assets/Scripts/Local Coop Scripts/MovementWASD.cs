using System.Collections;
using UnityEngine;

public class MovementWASD : PlayerMovement
{
    protected override Vector2 GetMovementDirection()
    {
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

        return movementDirection.normalized;
    }

    protected override Vector2 GetRawInput()
    {
        float xRaw = Input.GetAxisRaw("HorizontalWASD");
        float yRaw = Input.GetAxisRaw("VerticalWASD");
        return new Vector2(xRaw, yRaw);
    }
}