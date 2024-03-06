using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeMenu : MonoBehaviour
{
    public string nextSplit;
    public string nextNetworked;

    public void setSplitScreen()
    {
        GameManager.instance.SetNetworked(false);
        Screen.SetResolution(1280, 480, false);
        SceneManager.LoadScene("Tutorial 1");
    }

    public void setNetworked()
    {
        GameManager.instance.SetNetworked(true);
        Screen.SetResolution(640, 480, false);
        SceneManager.LoadScene("HostOrClient");
    }
}
