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

    public PlayerJump jump;
    public PlayerAnimation anim;

/*  public delegate void OnVariableChangeDelegate(bool newVal);
    public event OnVariableChangeDelegate OnVariableQLockChange;
    private bool m_qlocked = false;

    public bool qlocked
    {
        get { return m_qlocked; }
        set
        {
            if (m_qlocked == value) return;
            m_qlocked = value;
            if (OnVariableQLockChange != null) OnVariableQLockChange(m_qlocked);
        }
    }*/

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
    }

    public void SetPlayerNetworked()
    {
        Debug.Log($"Set PLayer Networked: {isActivePlayer} for {name}");
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
        // LevelManager lm = LevelManager.instance;
        LevelLoader ll = LevelLoader.instance;
        GameManager gm = GameManager.instance;
        PlayerMovement pm = GetComponent<PlayerMovement>();

        pm.canMove = false;

        if (gm.IsNetworked()) {
            ll.ReloadLevelServerRpc();
        } else {
            anim.SetTrigger("death");
            ll.ReloadLevel();
        }

        

        
    }
}
