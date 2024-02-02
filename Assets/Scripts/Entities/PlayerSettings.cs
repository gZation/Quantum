using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public bool world1 = false;
    public bool sceneStart = false;

    private void Start()
    {
        if (GameManager.instance.IsNetworked())
        {
            SetPlayerNetworked();
        } else
        {
            SetPlayerSplit();
        }
    }

    public void SetPlayerSplit()
    {
        // make the player have either WASD or arrow controls
        if (world1)
        {
            this.gameObject.AddComponent<MovementWASD>();
        } else
        {
            this.gameObject.AddComponent<MovementArrows>();
        }
    }

    public void SetPlayerNetworked()
    {
        // make the player have just base player movement
        // need to add this to check if it is the player that someone is playing
        
        if (world1 == GameManager.instance.networkManager.IsHost)
        {
            this.gameObject.AddComponent<PlayerMovement>();
        }
    }
}
