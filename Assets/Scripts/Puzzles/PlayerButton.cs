using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerButton : MonoBehaviour
{
    public UnityEvent onEnter;
    public Sprite down;
    public Sprite up;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            spriteRenderer.sprite = down;
            onEnter.Invoke();
        }   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            spriteRenderer.sprite = up;
        }
    }

}
