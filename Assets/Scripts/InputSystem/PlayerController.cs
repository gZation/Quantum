using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region References
    private PlayerInput m_playerInput;
    private InputAction ia_movement;
    
    // Player Related
    [SerializeField] private GameObject m_playerReference;
    private PlayerMovement pr_playerMovement;
    //private PlayerJump pr_playerJump;
    #endregion

    public GameObject PlayerReference
    {
        get
        {
           return m_playerReference;
        }
        set
        {
            m_playerReference = value;

            if (m_playerReference == null) return;

            pr_playerMovement = m_playerReference.GetComponent<PlayerMovement>();
            //pr_playerJump = m_playerReference.GetComponent<PlayerJump>();
        }
    }

    private void Awake()
    {
        // Setup references
        m_playerInput = GetComponent<PlayerInput>();

        // Connect Input Actions to C# Methods
        ia_movement = m_playerInput.actions["Move"];
        m_playerInput.actions["QuantumLock"].started += QuantumLockAction;
        m_playerInput.actions["Jump"].started += JumpAction;
        m_playerInput.actions["Dash"].started += DashAction;
        m_playerInput.actions["Pause"].started += PauseAction;

        // Enable controlls by default
        m_playerInput.actions.actionMaps[0].Enable(); // Player Map
    }

    private void Update()
    {
        MoveAction();
    }

    #region Actions
    private void MoveAction()
    {
        if (m_playerReference == null) return;

        pr_playerMovement.Move(ia_movement.ReadValue<Vector2>());
    }
    private void JumpAction(InputAction.CallbackContext context)
    {
        if (m_playerReference == null) return;

        pr_playerMovement.JumpLogic();
    }
    private void DashAction(InputAction.CallbackContext context)
    {
        if (m_playerReference == null) return;

        Vector2 dir = ia_movement.ReadValue<Vector2>();
        pr_playerMovement.Dash(dir.x, dir.y);
    }
    private void QuantumLockAction(InputAction.CallbackContext context)
    {
        if (m_playerReference == null) return;

        pr_playerMovement.QuantumLock();
    }
    private void PauseAction(InputAction.CallbackContext context)
    {
        PauseMenu.instance.TriggerPause();
    }
    #endregion
}
