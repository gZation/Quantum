using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    Vector3 playerStart;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.transform.parent == transform.parent.parent)
            {
                playerStart = p.transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.position = playerStart;
        }
    }
}