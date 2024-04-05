using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class PauseMenuManager : NetworkBehaviour
{
    public static PauseMenuManager instance { get; private set; }

    public delegate void OnVariableChangeDelegate(bool newVal);
    public event OnVariableChangeDelegate OnVariablePauseChange;
    private bool m_paused = false;
    public bool paused
    {
        get { return m_paused; }
        set
        {
            if (m_paused == value) return;
            m_paused = value;
            if (OnVariablePauseChange != null) OnVariablePauseChange(m_paused);
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    [ClientRpc]
    public void TogglePauseClientRpc() { TogglePause(); }

    [ServerRpc(RequireOwnership = false)]
    public void TogglePauseServerRpc() { TogglePauseClientRpc(); }

    // Does the pause variable automatically update between the two? like is that wat is happening?
    public void TogglePause()
    {
        instance.paused = !instance.paused;

        PauseMenu.instance.TogglePause(instance.paused);
    }

}
