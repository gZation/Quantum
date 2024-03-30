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
    public Vector3 newD = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        startTime = 0;
        direction = direction.normalized * magnitude;
    }

    void FixedUpdate()
    {
        float bufferDuration = oscillateDuration * 0.25f;
        Vector3 acc = new Vector3(0.0f, 0.0f, 0.0f);
        if (oscillate)
        {
            startTime += Time.deltaTime;

            if (startTime >= oscillateDuration)
            {
                startTime = 0;
                direction = -direction;
            }
            if (oscillateDuration - startTime < bufferDuration || startTime < bufferDuration)
            {
                acc = direction / bufferDuration;

                if (oscillateDuration - startTime < bufferDuration)
                {
                    newD = direction - acc * (bufferDuration - oscillateDuration + startTime);
                }
                else
                {
                    newD = acc * startTime;
                }
            }
            else
            {
                newD = direction;
            }
        }
        transform.position += newD;
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