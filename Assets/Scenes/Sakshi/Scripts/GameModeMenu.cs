using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeMenu : MonoBehaviour
{
    // I'm not sure if I should be setting this var for Game Manager's instance
    public void setSplitScreen()
    {
        GameManager.instance.networkingOn = false;
    }

    public void setNetworked()
    {
        GameManager.instance.networkingOn = true;
    }

}
