using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    private int playersSuccess;
    public string nextLevel;

    public LevelLoader levelLoader;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Manager in the scene.");
            Destroy(this.gameObject);
        }

        instance = this;
    }
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
    }
}
