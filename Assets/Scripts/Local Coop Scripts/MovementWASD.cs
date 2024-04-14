using System.Collections;
using UnityEngine;

public class MovementWASD : PlayerMovement
{
    protected override Vector2 GetMovementDirection()
    {
        if (!legacyInput) return Vector2.zero;
        return new Vector2(Input.GetAxis("HorizontalWASD"), Input.GetAxis("VerticalWASD"));
    }

    protected override Vector2 GetRawInput()
    {
        if (!legacyInput) return Vector2.zero;
        float xRaw = Input.GetAxisRaw("HorizontalWASD");
        float yRaw = Input.GetAxisRaw("VerticalWASD");
        return new Vector2(xRaw, yRaw);
    }
}