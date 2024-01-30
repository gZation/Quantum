using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalMomentumAddition : MonoBehaviour
{

    [SerializeField] Vector2 direction;
    [SerializeField] float strength;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.WorldAddMomentum(direction * strength);
        }
    }
}
