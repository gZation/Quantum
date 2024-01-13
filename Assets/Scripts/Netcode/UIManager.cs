using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TMP_InputField ipAddrField;

    private void Awake()
    {
        Cursor.visible = true;
    }

    public void Start() {
        

        startHostButton.onClick.AddListener(() => {
            updateIP();
            Debug.Log("Host started");
            if (NetworkManager.Singleton.StartHost()) {
                Logger.Instance.LogInfo("Host Started");
            } else {
                Logger.Instance.LogInfo("Host could not start");
            }
        });

        startServerButton.onClick.AddListener(() => {
            updateIP();
            if (NetworkManager.Singleton.StartServer()) {
                Logger.Instance.LogInfo("Server Started");
            } else {
                Logger.Instance.LogInfo("Server could not start");
            }
        });

        startClientButton.onClick.AddListener(() => {
            updateIP();
            if (NetworkManager.Singleton.StartClient()) {
                Logger.Instance.LogInfo($"Connecting to {NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address}");
                Logger.Instance.LogInfo("Client Started");
            } else {
                Logger.Instance.LogInfo("Client could not start");
            }
        });
    }

    private void updateIP() {
        string ipAddr = ipAddrField.text == "" ? "127.0.0.1" : ipAddrField.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ipAddr,
            7777);
    }

}
