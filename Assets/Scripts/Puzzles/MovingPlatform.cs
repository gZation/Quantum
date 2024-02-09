using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 direction;
    private float startTime;
    public bool oscillate = true;
    public float oscillateDuration = 2f;
    // Start is called before the first frame update
    void Start()
    {
        startTime = 0;
    }

    void FixedUpdate()
    {
        if (oscillate)
        {
            startTime += Time.deltaTime;
            if (startTime >= oscillateDuration)
            {
                startTime = 0;
                direction = -direction;
            }
        }
        transform.position += direction;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.parent = transform;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.parent = null;
        }
    }

}