using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] protected float fallMultiplier = 3f;
    [SerializeField] protected float lowJumpMultiplier = 2f;
    [SerializeField] protected float speed = 8;
    [SerializeField] protected float jumpForce = 8;
    [SerializeField] protected float slideSpeed = 5;
    [SerializeField] protected float dashSpeed = 20;

    [SerializeField] protected Vector2 bottomOffset = new Vector2(0, -0.65f);
    [SerializeField] protected Vector2 leftOffset = new Vector2(-0.45f, 0);
    [SerializeField] protected Vector2 rightOffset = new Vector2(0.45f, 0);
    [SerializeField] protected float collisionRadius = 0.25f;

    public bool grounded = false;
    public bool onWall = false;
    public bool onRightWall = false;
    public bool canMove = true;
    public bool wallJumped = false;
    public bool dashing = false;
    public bool canDash = true;

    public bool world1 = false;

    protected Rigidbody2D rb;
    protected Animator animator;
    public bool sharingMomentum { get; set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sharingMomentum = false;
    }

    public virtual void Update()
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
            canDash = true;
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
        } else if (onWall && !grounded && !wallJumped)
        {
            WallSlide();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuantumLock();
        }
    }

    protected void Walk(Vector2 direction)
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
        animator.SetFloat("speed", rb.velocity.x);
    }

    protected void Dash(float x, float y)
    {
        wallJumped = true;
        rb.velocity = Vector2.zero;
        Vector2 direction = new Vector2(x, y);

        Vector2 dashExtra = direction.normalized * dashSpeed;

        if (direction.y > 0)
        {
            dashExtra.y = dashExtra.y / 3;
        }

        rb.velocity += dashExtra;

        if (dashExtra.magnitude > 0)
        {
            StartCoroutine(DashWait());
        }


        if (sharingMomentum)
        {
            GameManager.instance.SendMomentum(dashExtra, this.gameObject);
        }

        canDash = false;
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

    protected void Jump(Vector2 direction)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += direction * jumpForce;

        if (sharingMomentum)
        {
            GameManager.instance.SendMomentum(direction.normalized * jumpForce, this.gameObject);
        }
    }

    protected void WallJump()
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

    protected void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    protected void QuantumLock()
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