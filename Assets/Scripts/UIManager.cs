using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    private void Awake()
    {
        Cursor.visible = true;
    }

    public void Start() {
        startHostButton.onClick.AddListener(() => {
            Debug.Log("Host started");
            if (NetworkManager.Singleton.StartHost()) {
                Logger.Instance.LogInfo("Host Started");
            } else {
                Logger.Instance.LogInfo("Host could not start");
            }
        });

        startServerButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartServer()) {
                Logger.Instance.LogInfo("Server Started");
            } else {
                Logger.Instance.LogInfo("Server could not start");
            }
        });

        startClientButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartClient()) {
                Logger.Instance.LogInfo("Client Started");
            } else {
                Logger.Instance.LogInfo("Client could not start");
            }
        });

    }

}
