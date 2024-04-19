using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using AddressFamily = System.Net.Sockets.AddressFamily;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ShowIPAddress : MonoBehaviour
{
    public TextMeshProUGUI ipAddressTextbox;
    public TextMeshProUGUI useDefaultTextbox;
    private string defaultIP;
    private void Start()
    {
        defaultIP = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address.ToString();
        ipAddressTextbox.text = defaultIP;
        useDefaultTextbox.text += "\n" + defaultIP;

        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipAddressTextbox.text = ip.ToString();
                return;
            }
        }

    }

    public void GoBack()
    {
        LevelLoader.instance.LoadLevelByName("HostOrClient", false);
    }

    public void ContinueWithInputIp()
    {
        ConnectToServer(ipAddressTextbox.text);
    }

    public void ContinueWithDefaultIP()
    {
        ConnectToServer(defaultIP);
    }

    private void ConnectToServer(string hostIP)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(hostIP, (ushort)7777);
        //NetworkManager.Singleton.StartHost();
        //NetworkManager.Singleton.LevelLoader.instance.LoadLevelByName("Tutorial 1", LoadSceneMode.Single);
        //PlayerManager.instance.isHost = true;
        LevelLoader.instance.LoadLevelByName("WaitingForPlayers", false);
    }
}
