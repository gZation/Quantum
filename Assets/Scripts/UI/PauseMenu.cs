using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu: MonoBehaviour
{
    public static PauseMenu instance { get; private set; }

    public static bool gamePaused = false;
    public GameObject pauseUI;
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

        // update the volume controls to match the music manager
        GameObject options = pauseUI.transform.GetChild(1).gameObject;
        Slider[] sliders = options.GetComponentsInChildren<Slider>();
        sliders[0].value = MusicManager.instance.masterVolume;
        sliders[1].value = MusicManager.instance.sfxVolume;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            TriggerPause();
        }
    }

    public void ToMainMenu()
    {
        LevelLoader.instance.LoadLevelByName("StartMenu");
        //SceneManager.LoadScene("StartMenu");
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

    public void Restart()
    {
        LevelLoader.instance.ReloadLevel();
    }

    public void TriggerPause()
    {
        if (GameManager.instance.IsNetworked())
        {
            PauseMenuManager.instance.TogglePauseClientRpc();
        } else
        {
            PauseMenuManager.instance.TogglePause();
        }
    }

    public void TogglePause(bool paused)
    {
        // If to show the correct pause menu

        if ((!GameManager.instance.IsNetworked() && world == 2)
        || (GameManager.instance.IsNetworked() && world == PlayerManager.instance.currPlayer))
        {
            if (paused != true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

}
