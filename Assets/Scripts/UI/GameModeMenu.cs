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
        GameManager.instance.SetNetworkedScreen(false);
        Screen.SetResolution(1280, 480, false);
        LevelLoader.instance.LoadLevelByName(nextSplit);
        //SceneManager.LoadScene(nextSplit);
    }

    public void setNetworked()
    {
        GameManager.instance.SetNetworkedScreen(true);
        Screen.SetResolution(640, 480, false);
        LevelLoader.instance.LoadLevelByName(nextNetworked, false);
        //SceneManager.LoadScene(nextNetworked);
    }
}
