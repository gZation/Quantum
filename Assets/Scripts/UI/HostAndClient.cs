using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostAndClient : MonoBehaviour
{
    public void ShowIPScreen()
    {
        PlayerManager.instance.isHost = true;
        SceneManager.LoadScene("CharacterSelect");
    }

    public void EnterHostIPScreen()
    {
        PlayerManager.instance.isHost = false;
        SceneManager.LoadScene("IP Input");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("SelectMode");
    }
}