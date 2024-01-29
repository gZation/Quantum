using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int playersSuccess;
    public string nextLevel;

    public LevelLoader levelLoader;

    void Start()
    {
        playersSuccess = 0;
    }

    public void AddPlayerSuccess()
    {
        playersSuccess++;

        if (playersSuccess == 2) {
            MoveOn();
        }
    }

    public void RemovePlayerSuccess()
    {
        playersSuccess--;
    }

    void MoveOn()
    {
        levelLoader.LoadLevelByName(nextLevel);
        GameManager.instance.LoadNextLevel();
    }
}
