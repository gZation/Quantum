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
    private void Start()
    {
        string defaultIP = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address.ToString();
        Debug.Log(defaultIP);
        ipAddressTextbox.text = defaultIP;
        useDefaultTextbox.text += "\n" + defaultIP;

        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        Debug.Log(hostEntry);
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
        SceneManager.LoadScene("HostOrClient");
    }

    public void ContinueWithInputIp()
    {

    }

    public void ContinueWithDefaultIP()
    {

    }
}
