using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public static bool gamePaused = false;
    public GameObject pauseUI;
    public int mainMenuIndex;
    public GameObject mainPause;
    public GameObject optionsMenu;
    public GameObject quitCheck;
    

    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gamePaused == true)
            {
                Resume();
            }else
            {
                Pause();
            }
        }
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(mainMenuIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        mainPause.SetActive(true);
        optionsMenu.SetActive(false);
        quitCheck.SetActive(false);
        gamePaused = false;
    }

    void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }
}
