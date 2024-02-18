using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 direction;
    public float magnitude;
    private float startTime;
    public bool oscillate = true;
    public float oscillateDuration = 2f;
    // Start is called before the first frame update
    void Start()
    {
        startTime = 0;
        direction = direction.normalized * magnitude;
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

    void OnDrawGizmosSelected()
    {
        Vector3 p1 = transform.position;
        Vector3 p2 = transform.position + (direction.normalized * magnitude * oscillateDuration * 100 / 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(p1, p2);
    }

}