using System.Collections;
using UnityEngine;

public class MovementWASD : PlayerMovement
{
    protected override Vector2 GetMovementDirection()
    {
        return new Vector2(Input.GetAxis("HorizontalWASD"), Input.GetAxis("VerticalWASD"));
    }

    protected override Vector2 GetRawInput()
    {
        float xRaw = Input.GetAxisRaw("HorizontalWASD");
        float yRaw = Input.GetAxisRaw("VerticalWASD");
        return new Vector2(xRaw, yRaw);
    }
}