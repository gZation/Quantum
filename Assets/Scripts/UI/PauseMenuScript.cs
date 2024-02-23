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
    public int world;
    

    private void Start()
    {
        Resume();

        RectTransform bg = pauseUI.GetComponentInChildren<RectTransform>();
        if (GameManager.instance.IsNetworked())
        {
            bg.sizeDelta = new Vector2(640, 480);
        } else
        {
            bg.sizeDelta = new Vector2(1280, 480);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if ((!GameManager.instance.IsNetworked() && world == 2)
                || (GameManager.instance.IsNetworked() && world == PlayerManager.instance.currPlayer))
            {
                if (gamePaused == true)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
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
