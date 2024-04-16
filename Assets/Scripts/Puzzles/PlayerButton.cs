using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerButton : NetworkBehaviour
{
    public UnityEvent onEnter;
    public Sprite down;
    public Sprite up;
    public Sprite down2;
    public Sprite up2;
    public bool changes;

    private bool one;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        one = true;
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
            this.GetComponent<NetworkObject>().ChangeOwnership((ulong)clientId);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (one)
            {
                spriteRenderer.sprite = down;
            } else
            {
                spriteRenderer.sprite = down2;
            }
            if (changes)
            {
                one = !one;
            }

            if (!GameManager.instance.IsNetworked()) { onEnter.Invoke(); }
            else { if (IsOwner) OnTriggerServerRpc(); }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void OnTriggerServerRpc() { if (IsHost) OnTriggerClientRpc(); }

    [ClientRpc]
    private void OnTriggerClientRpc() { onEnter.Invoke(); }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (one)
            {
                spriteRenderer.sprite = up;
            } else
            {
                spriteRenderer.sprite = up2;
            }
        }
    }

}
