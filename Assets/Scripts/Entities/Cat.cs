using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            animator.SetTrigger("out");
        }
    }

    public void MoveOn()
    {
        GameManager.instance.cutscene = true;
        LevelManager.instance.AddPlayerSuccess();
    }
}
