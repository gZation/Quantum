using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostAndClient : MonoBehaviour
{
    public void ShowIPScreen()
    {
        PlayerManager.instance.isHost = true;
        LevelLoader.instance.LoadLevelByName("CharacterSelection", false);
    }

    public void EnterHostIPScreen()
    {
        PlayerManager.instance.isHost = false;
        LevelLoader.instance.LoadLevelByName("IP Input", false);
    }

    public void GoBack()
    {
        LevelLoader.instance.LoadLevelByName("SelectMode", false);
    }
}