using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSettings : MonoBehaviour
{
    public bool world1 = false;
    public bool isActivePlayer = false;
    public bool locked = false;

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
        } else if (GameManager.instance.IsNetworked())
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
        if (isActivePlayer)
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

    public void Die()
    {
        LevelManager lm = LevelManager.instance;
        lm.Reload();
    }
}
