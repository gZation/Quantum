using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public bool world1 = false;
    //public bool sceneStart = false;

    public PlayerJump jump;
    public PlayerAnimation anim;

    private void Awake()
    {
        jump = GetComponent<PlayerJump>();
        anim = GetComponentInChildren<PlayerAnimation>();
    }


    void Start()
    {
        if (!GameManager.instance.IsNetworked())
        {
            SetPlayerSplit();
        } else if (GameManager.instance.IsNetworked() && PlayerManager.instance.isPlayersInit())
        {
            SetPlayerNetworked();
        }
    }

    public void SetPlayerSplit()
    {
        // make the player have either WASD or arrow controls
        if (world1)
        {
            this.gameObject.AddComponent<MovementWASD>();
        } else
        {
            this.gameObject.AddComponent<MovementArrows>();
        }
        UpdatePlayerMovementRef();
    }

    public void SetPlayerNetworked()
    {
        // make the player have just base player movement
        // need to add this to check if it is the player that someone is playing
        if (world1 == GameManager.instance.networkManager.IsHost)
        {
            this.gameObject.AddComponent<PlayerMovement>();
            UpdatePlayerMovementRef();
        }
    }

    public void UpdatePlayerMovementRef()
    {
        jump.SetMovementRef();
        anim.SetMovementRef();
    }
}
