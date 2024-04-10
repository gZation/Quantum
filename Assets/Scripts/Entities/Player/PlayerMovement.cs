using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerCollision coll;
    private PlayerSettings settings;
    [HideInInspector]
    public Rigidbody2D rb;
    private PlayerAnimation anim;

    [Space]
    [Header("Stats")]
    public float speed = 7;
    public float jumpForce = 12;
    public float maxSlideSpeed = 6;
    public float minSlideSpeed = 1.5f;
    public float accelerationFactor = 2f;
    public float wallJumpLerp = 5;
    public float dashSpeed = 25;
    public int world;

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
    private Camera camera;
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;
    ParticleSystem.MinMaxGradient slideColor;

    public float currentSlide;

    // Start is called before the first frame update
    void Start()
    {
        settings = GetComponent<PlayerSettings>();
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<PlayerAnimation>();
        momentumToAdd = new List<Vector2>();

        slideParticle.Play();
        slideColor = slideParticle.main.startColor;

        currentSlide = minSlideSpeed;

        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");

        foreach (GameObject c in cameras)
        {
            Camera p_camera = c.GetComponent<Camera>();
            int worldlayer = LayerMask.NameToLayer("World" + world);

            if (c.layer == worldlayer)
            {
                camera = p_camera;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // make sure the player rb doesnt go to sleep
        rb.AddForce(Vector2.zero);

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

        rb.gravityScale = 3;

        if (coll.onWall && !coll.onGround)
        {
            wallSlide = true;
            WallSlide();
        }

        if (!coll.onWall || coll.onGround)
        {
            currentSlide = minSlideSpeed;
            wallSlide = false;
        }

        if (IsJump() && canMove)
        {
            anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround)

                WallJump();
        }

        if (IsDash() && !hasDashed && canMove)
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

        WallParticle(y);

        if (wallGrab || wallSlide || !canMove)
            return;

        if (x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }

        //check speed and cap at max speed
        Vector2 vel = rb.velocity;
        if (vel.magnitude > speed * 4)
        {
            vel = vel.normalized;
            vel = vel * speed * 4;
            rb.velocity = vel;
        }
    }

    protected void FixedUpdate()
    {
        AddMomentum();

        Vector2 vel = rb.velocity;
        if (vel.magnitude > speed * 7)
        {
            vel = vel.normalized;
            vel = vel * speed * 7;
            rb.velocity = vel;
        }
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

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        StartCoroutine(camera.GetComponent<CameraShake>().Shake(0.1f, 0.4f));

        hasDashed = true;

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        Vector2 dashExtra = dir.normalized * dashSpeed;

        if (dir.y > 0)
        {
            dashExtra.y = dashExtra.y / 1.9f;
        }
        else
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

        dashExtra.x *= 4f;
        PlayerManager.instance.SendMomentum(dashExtra, this.gameObject);

        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());

        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<PlayerJump>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(0.1f);

        dashParticle.Stop();
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


        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;


        Jump((Vector2.up / 1.4f + wallDir / 1.2f), true);


        wallJumped = true;

    }  
        
   
    private void WallSlide()
    {
        if (coll.wallSide != side)
            anim.Flip(side * -1);

        if (!canMove)
            return;
        
        bool pushingWall = false;

        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }

        float push = pushingWall ? 0 : rb.velocity.x;

        currentSlide = Math.Min(currentSlide + accelerationFactor * Time.deltaTime, maxSlideSpeed);
        float slideVelocity = -currentSlide;

        rb.velocity = new Vector2(push, slideVelocity);

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
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        //too op nerfing it for now
        /*        if (sharingMomentum)
                {
                    PlayerManager.instance.SendMomentum(dir * jumpForce, this.gameObject);
                    DisableLocking(.2f);
                }*/

        particle.Play();
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

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide)
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = slideColor;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    public void QuantumLock()
    {
        PlayerManager.instance.ToggleQuantumLock();
    }

    public void QuantumLockAddMomentum(Vector2 momentum)
    {
        momentumToAdd.Add(momentum);
    }

    public void WorldAddMomentum(Vector2 momentum)
    {
        momentumToAdd.Add(momentum);

        PlayerManager.instance.SendMomentum(momentum, this.gameObject);
    }

    protected void AddMomentum()
    {
        foreach (Vector2 momentum in momentumToAdd)
        {
            rb.velocity += momentum;
        }

        momentumToAdd = new List<Vector2>();
    }
}