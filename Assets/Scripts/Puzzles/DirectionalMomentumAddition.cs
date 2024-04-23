using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DirectionalMomentumAddition : MonoBehaviour
{
    public VisualEffect VFX;
    [SerializeField] Vector2 direction;
    [SerializeField] float strength;
    [SerializeField] bool active;
    Animator animator;

    public bool isWind;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("active", active);
        }
        if (VFX != null)
        {
            VFX.enabled = active;
        }

        if (isWind && active)
        {
            MusicManager.instance.Play("Wind");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void toggleActive()
    {
        active = !active;
        if (animator != null)
        {
            animator.SetBool("active", active);
        }
        if (VFX != null)
        {
            VFX.enabled = active;
        }

        if (isWind && active)
        {
            MusicManager.instance.Play("Wind");
        } else if (isWind)
        {
            MusicManager.instance.Stop("Wind");
        }
    }

    public void Off()
    {
        active = false;
        if (animator != null)
        {
            animator.SetBool("active", active);
        }
        if (VFX != null)
        {
            VFX.enabled = active;
        }

        if (isWind)
        {
            MusicManager.instance.Stop("Wind");
        }
    }

    public void On()
    {
        active = true;
        if (animator != null)
        {
            animator.SetBool("active", active);
        }
        if (VFX != null)
        {
            VFX.enabled = active;
        }

        if (isWind)
        {
            MusicManager.instance.Play("Wind");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (active && collision.gameObject.tag == "Player")
        {
            if (GameManager.instance.IsNetworked() && collision.gameObject != PlayerManager.instance.currPlayerObject) return;
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.canSlide = false;
            player.WorldAddMomentum(direction * strength);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (active && collision.gameObject.tag == "Player")
        {
            if (GameManager.instance.IsNetworked() && collision.gameObject != PlayerManager.instance.currPlayerObject) return;
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.canSlide = true;
        }
    }
}
