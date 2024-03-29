using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]

    public Vector2 boxSize = new Vector2(0, 0);
    public Vector2 bottomBoxSize = new Vector2(0, 0);
    public Vector2 bottomOffset, rightOffset, leftOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomBoxSize, 0, groundLayer);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, boxSize, 0, groundLayer)
            || Physics2D.OverlapBox((Vector2)transform.position + leftOffset, boxSize, 0, groundLayer);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, boxSize, 0, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, boxSize, 0, groundLayer);

        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, new Vector3(bottomBoxSize.x, bottomBoxSize.y, 1));
        Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, new Vector3(boxSize.x, boxSize.y, 1));
        Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, new Vector3(boxSize.x, boxSize.y, 1));
    }
}