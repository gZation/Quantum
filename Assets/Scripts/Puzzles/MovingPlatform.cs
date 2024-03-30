using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovingPlatform : NetworkBehaviour
{
    public Vector3 direction;
    public float magnitude;
    private float startTime;
    public bool oscillate = true;
    public float oscillateDuration = 2f;
    public Vector3 newD = new Vector3(0.0f, 0.0f, 0.0f);
    private GameObject playerObject;
    private Vector3 playerOffset;

    // Start is called before the first frame update
    void Start()
    {
        startTime = 0;
        direction = direction.normalized * magnitude;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) return;

        //Connected client always has id of 1 (host is 0)
        int clientId = 1;

        //Check if curr moving platform is under Host's world grid level. If not, give ownership to client. 
        string hostWorldLevelTag = "World" + PlayerManager.instance.currPlayer + "Level";
        GameObject hostWorldGridLevel = GameObject.FindGameObjectWithTag(hostWorldLevelTag);
        if (transform.parent != hostWorldGridLevel.transform)
        {
            this.GetComponent<NetworkObject>().ChangeOwnership((ulong) clientId);
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance.IsNetworked() && !IsOwner) return;
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

        if (playerObject != null) playerObject.transform.position += newD;
    }

    // Can't reparent NetworkObjects in splitscreen, so manually adjust player position
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerObject = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerObject = null;
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