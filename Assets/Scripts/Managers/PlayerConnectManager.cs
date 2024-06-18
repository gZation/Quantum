using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerConnectManager : NetworkBehaviour
{
    // Start is called before the first frame update
    public float clientRetryDelay = (float) 0.5;
    public string NextLevel = "Tutorial 1";
    void Start()
    {
        if (!PlayerManager.instance.isHost)
        {
            Debug.Log("Client starting");
            NetworkManager.Singleton.StartClient();
            //GameManager.instance.SetSceneLoad();
        }
        else
        {
            Debug.Log("Host starting");
            NetworkManager.Singleton.StartHost();
            //GameManager.instance.SetSceneLoad();
        }
        //This has to occur after StartHost() and StartClient()
        NetworkManager.Singleton.OnClientConnectedCallback += LoadNextLevel;
        NetworkManager.Singleton.OnClientDisconnectCallback += GameManager.instance.BackToMainMenu;
    }

    private void LoadNextLevel(ulong clientId)
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= LoadNextLevel;
        GameManager.instance.GameEnable();
        LevelLoader.instance.LoadLevelByName(NextLevel);
        MusicManager.instance.StartLevelMusic();
        PlayerManager.instance.EnableControllerJoin();
    }
}
