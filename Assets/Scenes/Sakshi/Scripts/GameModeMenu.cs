using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeMenu : MonoBehaviour
{
    public string nextSplit;
    public string nextNetworked;

    public LevelLoader levelLoader;
    public void setSplitScreen()
    {
        GameManager.instance.SetNetworked(false);
        levelLoader.LoadLevelByName(nextSplit);
    }

    public void setNetworked()
    {
        GameManager.instance.SetNetworked(true);
        levelLoader.LoadLevelByName(nextNetworked);
    }
}
