using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float slideSpeed = 5;
    [SerializeField] private float dashSpeed = 5;

    [SerializeField] private Vector2 bottomOffset = new Vector2(0, -0.45f);
    [SerializeField] private Vector2 leftOffset = new Vector2(-0.45f, 0);
    [SerializeField] private Vector2 rightOffset = new Vector2(0.45f, 0);
    [SerializeField] private float collisionRadius = 0.25f;

    public bool grounded = false;
    public bool onWall = false;
    public bool onRightWall = false;
    public bool canMove = true;
    public bool wallJumped = false;
    public bool dashing = false;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        //if falling
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        Vector2 movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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

        if (onWall && !grounded)
        {
            WallSlide();
        }

        if (Input.GetButtonDown("Jump")) 
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

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }

    private void Walk(Vector2 direction)
    {
        if (!canMove)
        {
            return;
        }

        if (!wallJumped)
        {
            rb.velocity = (new Vector2(direction.x * speed, rb.velocity.y));
        } else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(direction.x * speed, rb.velocity.y)), 10 * Time.deltaTime);
        }
    }

    private void Dash(float x, float y)
    {
        wallJumped = true;
        rb.velocity = Vector2.zero;
        Vector2 direction = new Vector2(x, y);
        rb.velocity += direction.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }
    IEnumerator DashWait()
    {
        dashing = true;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(.3f);
        rb.gravityScale = 1;
        wallJumped = false;
        dashing = false;
    }

    private void Jump(Vector2 direction)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += direction * jumpForce;
    }

    private void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;
        Jump((Vector2.up / 0.1f + wallDir / 1.5f));
        wallJumped = true;
    }

    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
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