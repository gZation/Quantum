using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerConnectManager : NetworkBehaviour
{
    // Start is called before the first frame update
    public float clientRetryDelay = (float) 0.5;
    void Start()
    {
        
        Debug.Log(PlayerManager.instance.isHost);
        if (!PlayerManager.instance.isHost)
        {
            Debug.Log("Client starting");
            NetworkManager.Singleton.StartClient();
            GameManager.instance.SetSceneLoad();
        }
        else
        {
            Debug.Log("Host starting");
            NetworkManager.Singleton.StartHost();
            GameManager.instance.SetSceneLoad();
        }
        //This has to occur after StartHost() and StartClient()
        NetworkManager.Singleton.OnClientConnectedCallback += LoadNextLevel;
    }

    private void LoadNextLevel(ulong clientId)
    {
        Debug.Log($"Load next Level for client {clientId}");
        NetworkManager.Singleton.OnClientConnectedCallback -= LoadNextLevel;
        GameManager.instance.GameEnable();
        NetworkManager.Singleton.SceneManager.LoadScene("Tutorial 1", LoadSceneMode.Single);
    }
}
