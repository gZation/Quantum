using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : NetworkBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public static LevelLoader instance;


    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReloadLevelServerRpc() { ReloadLevel(); }

    public void ReloadLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadLevelByName(string name)
    {
        StartCoroutine(LoadLevel(name));
    }

    public void LoadLevelByName(string name, bool check)
    {
        if (check)
        {
            StartCoroutine(LoadLevel(name));
        } else
        {
            StartCoroutine(LoadLevelNoCheckNetworked(name));
        }
    }

    public void LoadLoseScreen()
    {
        StartCoroutine(LoadLevel("Lose Screen"));
    }

    // Coroutines
    IEnumerator LoadLevel<T>(T levelIndex)
    {
        T newIndex = levelIndex;
        // Play animation
        transition.SetTrigger("Start");
        // Wait
        Time.timeScale = 1f;
        yield return new WaitForSeconds(transitionTime);
        // if T is string, convert levelIndex from T type to string
        // sometimes will return build index -1 if the scene hasn't been loaded before so :<
        string sceneName = (typeof(T) == typeof(string)) ? (string)(object)levelIndex : SceneManager.GetSceneByBuildIndex((int)(object)(levelIndex)).name;

        // Use NetworkSceneManager if networked. Otherwise, revert to normal SceneManager
        //print($"LL Spawned? {IsSpawned}");
        //print($"{NetworkManager.Singleton.IsHost}, {IsClient}");
        //print($"Load level {GameManager.instance != null}, {GameManager.instance.IsNetworked()}, {NetworkManager.Singleton.IsHost}");
        if (GameManager.instance != null && GameManager.instance.IsNetworked())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
        
    }

    IEnumerator LoadLevelNoCheckNetworked<T>(T levelIndex)
    {
        T newIndex = levelIndex;
        // Play animation
        transition.SetTrigger("Start");
        // Wait
        Time.timeScale = 1f;
        yield return new WaitForSeconds(transitionTime);
        // if T is string, convert levelIndex from T type to string
        // sometimes will return build index -1 if the scene hasn't been loaded before so :<
        string sceneName = (typeof(T) == typeof(string)) ? (string)(object)levelIndex : SceneManager.GetSceneByBuildIndex((int)(object)(levelIndex)).name;

        SceneManager.LoadScene(sceneName);
    }
}
