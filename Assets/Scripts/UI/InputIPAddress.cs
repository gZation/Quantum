using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InputIPAddress : MonoBehaviour
{
    public TMP_InputField ipInput;
    public void IPEnter()
    {
        string hostIP = ipInput.text;
        Debug.Log($"Host IP: {hostIP}");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("HostOrClient");
    }
}
