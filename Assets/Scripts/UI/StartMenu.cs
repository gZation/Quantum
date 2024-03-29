using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    public void StartGame() {
        LevelLoader.instance.LoadLevelByName("SelectMode");
    }

    public void ExitGame() {
        Application.Quit();
    }
}
