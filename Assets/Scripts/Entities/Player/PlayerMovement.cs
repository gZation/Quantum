using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerCollision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private PlayerAnimation anim;

    [Space]
    [Header("Stats")]
    public float speed = 8;
    public float jumpForce = 15;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 25;

    [Space]
    [Header("Booleans")]
    public bool canMove = true;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;


    [Space]
    [Header("Quantum Locking")]
    private List<Vector2> momentumToAdd;
    public bool sharingMomentum;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<PlayerAnimation>();
        momentumToAdd = new List<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsQLock())
        {
            QuantumLock();
        }

        Vector2 dir = GetMovementDirection();
        float x = dir.x;
        float y = dir.y;
        Vector2 raw = GetRawInput();
        float xRaw = raw.x;
        float yRaw = raw.y;

        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.velocity.y);


        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<PlayerJump>().enabled = true;
        }

        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        if (IsJump())
        {
            anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround)
                WallJump();
        }

        if (IsDash() && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

/*        WallParticle(y);*/

        if (wallGrab || wallSlide || !canMove)
            return;

/*        if (x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }*/


    }

    protected void FixedUpdate()
    {

        AddMomentum();
    }

    protected virtual Vector2 GetMovementDirection()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    protected virtual Vector2 GetRawInput()
    {
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        return new Vector2(xRaw, yRaw);
    }

    public virtual bool IsJump()
    {
        return Input.GetButtonDown("Jump");
    }

    public virtual bool IsDash()
    {
        return Input.GetButtonDown("Fire1");
    }

    public virtual bool IsQLock()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        //side = anim.sr.flipX ? -1 : 1;

        // jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        /*Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));*/

        hasDashed = true;

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        Vector2 dashExtra = dir.normalized * dashSpeed;

        if (dir.y > 0)
        {
            dashExtra.y = dashExtra.y / 1.9f;
        } else
        {
            dashExtra *= 1.4f;
        }
        
        if (dir.y > 0 && dir.x == 0)
        {
            anim.SetTrigger("dashup");
        }
        else
        {
            anim.SetTrigger("dash");
        }

        rb.velocity += dashExtra;

        if (sharingMomentum)
        {
            dashExtra.x *= 4f;
            GameManager.instance.SendMomentum(dashExtra, this.gameObject);
        }

        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
      //FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());

        //dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<PlayerJump>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(0.45f);

        //dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<PlayerJump>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
/*        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }*/

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.4f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
/*        if (coll.wallSide != side)
            anim.Flip(side * -1);*/

        if (!canMove)
            return;

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        //slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        //ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        if (sharingMomentum)
        {
            GameManager.instance.SendMomentum(dir * jumpForce, this.gameObject);
            DisableLocking(.2f);
        }

        //particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerable DisableLocking(float time)
    {
        sharingMomentum = false;
        yield return new WaitForSeconds(time);
        sharingMomentum = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

 /*   void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }*/

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    protected void QuantumLock()
    {
        GameManager.instance.QuantumLockPlayer(this.gameObject);
    }

    public void QuantumLockAddMomentum(Vector2 momentum)
    {
        momentumToAdd.Add(momentum);
    }

    public void WorldAddMomentum(Vector2 momentum)
    {
        momentumToAdd.Add(momentum);

        if (sharingMomentum)
        {
            GameManager.instance.SendMomentum(momentum, this.gameObject);
        }
    }

    protected void AddMomentum()
    {
        foreach (Vector2 momentum in momentumToAdd)
        {
            /*rb.AddForce(momentum, ForceMode2D.Impulse);*/
            rb.velocity += momentum;
        }

        momentumToAdd = new List<Vector2>();
    }
}