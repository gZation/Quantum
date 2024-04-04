using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
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

}
