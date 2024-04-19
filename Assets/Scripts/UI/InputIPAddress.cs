using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class InputIPAddress : MonoBehaviour
{
    public TMP_InputField ipInput;
    public TextMeshProUGUI placeholder;
    private string defaultIP;

    private void Start()
    {
        defaultIP = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
        placeholder.text = defaultIP;
    }

    public void IPEnter()
    {
        string hostIP = ipInput.text == "" ? defaultIP : ipInput.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(hostIP, (ushort)7777);
        //Debug.Log($"Connecting to host with IP: {hostIP}");
        //PlayerManager.instance.isHost = false;
        LevelLoader.instance.LoadLevelByName("WaitingForPlayers", false);
    }

    public void GoBack()
    {
        LevelLoader.instance.LoadLevelByName("HostOrClient", false);
    }
}
