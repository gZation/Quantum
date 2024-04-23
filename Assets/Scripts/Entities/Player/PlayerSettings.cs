using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSettings : MonoBehaviour
{
    public bool world1 = false; // if world1 = player1 :D
    public bool isActivePlayer = false;

    public PlayerJump jump;
    public PlayerAnimation anim;

    private void Awake()
    {
        jump = GetComponent<PlayerJump>();
        anim = GetComponentInChildren<PlayerAnimation>();
    }

/*    void Start()
    {
        if (!GameManager.instance.IsNetworked())
        {
            // check if controlelrs?
            SetPlayerSplit();
        }
        else if (GameManager.instance.IsNetworked())
        {
            SetPlayerNetworked();
        }
    }*/

/*
    public void SetPlayerSplit()
    {
        int world = this.world1 ? 1 : 2;

        // make the player have either WASD or arrow controls
        if (PlayerManager.instance.playerOnLeft == world)
        {
            Destroy(this.gameObject.GetComponent<PlayerMovement>());
            Destroy(this.gameObject.GetComponent<MovementArrows>());
        } else
        {
            Destroy(this.gameObject.GetComponent<PlayerMovement>());
            Destroy(this.gameObject.GetComponent<MovementWASD>());
        }
        UpdatePlayerMovementRef();
    } */

/*    public void SetPlayerControllerSplit()
    {
        int world = this.world1 ? 1 : 2;

        // make the player have controllers
        if (PlayerManager.instance.playerOnLeft == world)
        {
            Destroy(this.gameObject.GetComponent<MovementWASD>());
            Destroy(this.gameObject.GetComponent<MovementArrows>());
        }
        else
        {
            Destroy(this.gameObject.GetComponent<MovementArrows>());
            Destroy(this.gameObject.GetComponent<MovementWASD>());
        }
        UpdatePlayerMovementRef();
    }*/

/*    public void SetPlayerNetworked()
    {
        if (isActivePlayer)
        {
            Destroy(this.gameObject.GetComponent<MovementWASD>());
            Destroy(this.gameObject.GetComponent<MovementArrows>());
            UpdatePlayerMovementRef();
        } else
        {
            Destroy(this.gameObject.GetComponent<MovementWASD>());
            Destroy(this.gameObject.GetComponent<MovementArrows>());
            Destroy(this.gameObject.GetComponent<PlayerMovement>());
           // this.gameObject.AddComponent<PlayerSpriteUpdater>();
            UpdatePlayerMovementRef();
        }
    }

    public void UpdatePlayerMovementRef()
    {
        jump.SetMovementRef();
        anim.SetMovementRef();
    }*/

    public void Die()
    {
        LevelLoader ll = LevelLoader.instance;
        GameManager gm = GameManager.instance;
        PlayerMovement pm = GetComponent<PlayerMovement>();

        pm.canMove = false;

        if (gm.IsNetworked()) {
            ll.ReloadLevelServerRpc();
        } else {
            ll.ReloadLevel();
        }

        anim.SetTrigger("death");
    }
}
