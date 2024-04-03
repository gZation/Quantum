using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrows : PlayerMovement
{
    protected override Vector2 GetMovementDirection()
    {
        return new Vector2(Input.GetAxis("HorizontalArrows"), Input.GetAxis("VerticalArrows"));
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