using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using AddressFamily = System.Net.Sockets.AddressFamily;
using TMPro;
using UnityEngine.SceneManagement;

public class IPAddress : MonoBehaviour
{
    public TextMeshProUGUI ipAddressTextbox;
    private string publicIPAddress;
    private void Start()
    {
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
        ipAddressTextbox.text = "localhost";
    }

    public void GoBack()
    {
        SceneManager.LoadScene("HostOrClient");
    }

    public void ContinueAsHost()
    {

    }
}
