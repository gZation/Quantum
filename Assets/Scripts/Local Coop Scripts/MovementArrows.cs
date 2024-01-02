using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrows : MonoBehaviour
{
    [SerializeField] private float fallMultiplier = 3f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float speed = 8;
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float slideSpeed = 5;
    [SerializeField] private float dashSpeed = 20;

    [SerializeField] private Vector2 bottomOffset = new Vector2(0, -0.65f);
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
    Animator animator;
    public bool sharingMomentum { get; set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sharingMomentum = false;
    }

    //Arrow to move,Keypad 0 to dash, right ctrl to jump, right shift to quantum lock
    void Update()
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

    private void Walk(Vector2 direction)
    {
        if (!canMove)
        {
            return;
        }

        if (!wallJumped)
        {
            rb.velocity = (new Vector2(direction.x * speed, rb.velocity.y));
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(direction.x * speed, rb.velocity.y)), 10 * Time.deltaTime);
        }
        animator.SetFloat("speed", rb.velocity.x);
    }

    private void Dash(float x, float y)
    {
        wallJumped = true;
        rb.velocity = Vector2.zero;
        Vector2 direction = new Vector2(x, y);
        rb.velocity += direction.normalized * dashSpeed;
        StartCoroutine(DashWait());

        if (sharingMomentum)
        {
            GameManager.instance.SendMomentum(direction.normalized * dashSpeed, this.gameObject);
        }
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

        if (sharingMomentum)
        {
            GameManager.instance.SendMomentum(direction.normalized * jumpForce, this.gameObject);
        }
    }

    private void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;
        Jump((Vector2.up / 2f + wallDir / 0.4f));
        StartCoroutine(WallJumpWait());
    }

    IEnumerator WallJumpWait()
    {
        wallJumped = true;
        yield return new WaitForSeconds(.2f);
        wallJumped = false;
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
    private void QuantumLock()
    {
        GameManager.instance.QuantumLockPlayer(this.gameObject);
    }

    public void AddMomentum(Vector2 momentum)
    {
        rb.velocity += momentum;
        StartCoroutine(DisableMovement(.2f));
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