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
    [SerializeField] private PlayerSettings pr_playerSettings;
    [SerializeField] private PlayerMovement pr_playerMovement;
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
            pr_playerSettings = m_playerReference.GetComponent<PlayerSettings>();
            pr_playerMovement = m_playerReference.GetComponent<PlayerMovement>();
            //pr_playerJump = m_playerReference.GetComponent<PlayerJump>();
        }
    }
    public PlayerMovement PlayerMovementRef
    {
        set
        {
            if (m_playerReference == null) return;
            pr_playerMovement = value;
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
        m_playerInput.actions["Restart"].started += RestartAction;
        m_playerInput.actions["Overlay"].started += OverlayAction;

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
        if (pr_playerMovement == null) pr_playerMovement = m_playerReference.GetComponent<PlayerMovement>();
        pr_playerMovement.Move(ia_movement.ReadValue<Vector2>());
    }
    private void JumpAction(InputAction.CallbackContext context)
    {
        if (m_playerReference == null) return;
        if (pr_playerMovement == null) pr_playerMovement = m_playerReference.GetComponent<PlayerMovement>();
        pr_playerMovement.JumpLogic();
    }
    private void DashAction(InputAction.CallbackContext context)
    {
        if (m_playerReference == null) return;
        if (pr_playerMovement == null) pr_playerMovement = m_playerReference.GetComponent<PlayerMovement>();
        Vector2 dir = ia_movement.ReadValue<Vector2>();
        pr_playerMovement.Dash(dir.x, dir.y);
    }
    private void QuantumLockAction(InputAction.CallbackContext context)
    {
        if (m_playerReference == null) return;
        if (pr_playerMovement == null) pr_playerMovement = m_playerReference.GetComponent<PlayerMovement>();
        pr_playerMovement.QuantumLock();
    }
    private void PauseAction(InputAction.CallbackContext context)
    {
        PauseMenu.instance.TriggerPause();
    }
    private void RestartAction(InputAction.CallbackContext context)
    {
        LevelLoader.instance.ReloadLevel();
    }
    private void OverlayAction(InputAction.CallbackContext context)
    {
        if (m_playerReference == null) return;
        if (pr_playerSettings == null) pr_playerSettings = m_playerReference.GetComponent<PlayerSettings>();
        GameManager.instance.ToggleOverlay(pr_playerSettings.world1);
    }
    #endregion
}
