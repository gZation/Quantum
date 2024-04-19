using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu: MonoBehaviour
{
    public static PauseMenu instance { get; private set; }

    public static bool gamePaused = false;
    public GameObject pauseUI1;
    public GameObject controlsUI1;
    public GameObject mainPause1;
    public GameObject optionsMenu1;
    public GameObject quitCheck1;

    public GameObject pauseUI2;
    public GameObject controlsUI2;
    public GameObject mainPause2;
    public GameObject optionsMenu2;
    public GameObject quitCheck2;
    public bool quit = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            //Debug.LogError("Found more than one Player Manager in the scene.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        Resume();

        // Start both pause menus
        RectTransform bg1 = pauseUI1.GetComponentInChildren<RectTransform>();
        if (GameManager.instance.IsNetworked())
        {
            bg1.sizeDelta = new Vector2(640, 480);
        } else
        {
            bg1.sizeDelta = new Vector2(1280, 480);
        }

        // update the volume controls to match the music manager
        GameObject options = pauseUI1.transform.GetChild(1).gameObject;
        Slider[] sliders = options.GetComponentsInChildren<Slider>();
        sliders[0].value = MusicManager.instance.masterVolume;
        sliders[1].value = MusicManager.instance.sfxVolume;

        RectTransform bg2 = pauseUI2.GetComponentInChildren<RectTransform>();
        if (GameManager.instance.IsNetworked())
        {
            bg2.sizeDelta = new Vector2(640, 480);
        }
        else
        {
            bg2.sizeDelta = new Vector2(1280, 480);
        }

        // update the volume controls to match the music manager
        GameObject options2 = pauseUI2.transform.GetChild(1).gameObject;
        Slider[] sliders2 = options2.GetComponentsInChildren<Slider>();
        sliders2[0].value = MusicManager.instance.masterVolume;
        sliders2[1].value = MusicManager.instance.sfxVolume;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            TriggerPause();
        }
    }

    public void ForceOpenControls()
    {
        if (GameManager.instance.IsNetworked())
        {
            PauseMenuManager.instance.OpenControlsServerRpc();
        }
        else
        {
            PauseMenuManager.instance.OpenControls();
        }
    }

    public void OpenControls()
    {
        if (GameManager.instance.IsNetworked())
        {
            if (PlayerManager.instance.currPlayer == 1)
            {
                controlsUI1.SetActive(true);
            }
            else
            {
                controlsUI2.SetActive(true);
            }
        }
        else
        {
            if (PlayerManager.instance.playerOnLeft == 2)
            {
                controlsUI2.SetActive(true);
            }
            else
            {
                controlsUI1.SetActive(true);
            }
        }
    }

    public void TriggerToMainMenu()
    {
        if (GameManager.instance.IsNetworked())
        {
            PauseMenuManager.instance.ToMainMenuServerRPC();
        }
        else
        {
            ToMainMenu();
        }
    }

    public void ToMainMenu()
    {
        LevelLoader.instance.LoadLevelByName("StartMenu");
    }

    public void TriggerQuit()
    {
        quit = true;
        if (GameManager.instance.IsNetworked())
        {
            PauseMenuManager.instance.QuitServerRPC();
        }
        else
        {
            Quit();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        if (GameManager.instance.IsNetworked())
        {
            if (PlayerManager.instance.currPlayer == 1)
            {
                pauseUI1.SetActive(false);
                Time.timeScale = 1f;
                mainPause1.SetActive(true);
                optionsMenu1.SetActive(false);
                controlsUI1.SetActive(false);

                quitCheck1.SetActive(false);
                gamePaused = false;
            } else
            {
                pauseUI2.SetActive(false);
                Time.timeScale = 1f;
                mainPause2.SetActive(true);
                optionsMenu2.SetActive(false);
                controlsUI2.SetActive(false);
                quitCheck2.SetActive(false);
                gamePaused = false;
            }
        } else
        {

            if (PlayerManager.instance.playerOnLeft == 2)
            {
                pauseUI2.SetActive(false);
                Time.timeScale = 1f;
                mainPause2.SetActive(true);
                optionsMenu2.SetActive(false);
                controlsUI2.SetActive(false);
                quitCheck2.SetActive(false);
                gamePaused = false;
            } else
            {
                pauseUI1.SetActive(false);
                Time.timeScale = 1f;
                mainPause1.SetActive(true);
                optionsMenu1.SetActive(false);
                controlsUI1.SetActive(false);
                quitCheck1.SetActive(false);
                gamePaused = false;
            }
        }
    }

    public void TriggerRestart()
    {
        if (GameManager.instance.IsNetworked())
        {
            PauseMenuManager.instance.RestartServerRpc();
        }
        else
        {
            Restart();
        }
    }

    public void Restart()
    {
        LevelLoader.instance.ReloadLevel();
    }

    public void TriggerPause()
    {
        if (GameManager.instance.IsNetworked())
        {
            PauseMenuManager.instance.TogglePauseServerRpc();
        } else
        {
            PauseMenuManager.instance.TogglePause();
        }
    }

    public void TogglePause(bool paused)
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

    public void Pause()
    {

        if (GameManager.instance.IsNetworked())
        {
            if (PlayerManager.instance.currPlayer == 1)
            {
                pauseUI1.SetActive(true);
            }
            else
            {
                pauseUI2.SetActive(true);
            }
        }
        else
        {
            if (PlayerManager.instance.playerOnLeft == 2)
            {
                pauseUI2.SetActive(true);
            } else
            {
                pauseUI1.SetActive(true);
            }
        }
        Time.timeScale = 0f;
        gamePaused = true;
    }

}
