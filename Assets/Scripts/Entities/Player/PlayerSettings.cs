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

    public delegate void OnVariableChangeDelegate(bool newVal);
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
    }

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
