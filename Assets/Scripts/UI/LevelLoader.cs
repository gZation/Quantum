using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : NetworkBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        yield return new WaitForSeconds(transitionTime);

        // if T is string, convert levelIndex from T type to string
        string sceneName = (typeof(T) == typeof(string)) ? (string)(object)levelIndex : SceneManager.GetSceneByBuildIndex((int)(object)(levelIndex)).name;

        // Use NetworkSceneManager if networked. Otherwise, revert to normal SceneManager
        if (GameManager.instance.IsNetworked() && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Change Scene Networked");
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        SceneManager.LoadScene(sceneName);
    }
}
