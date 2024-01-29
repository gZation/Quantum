using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
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
        if (typeof(T) == typeof(string))
        {
            SceneManager.LoadScene((string)(object)levelIndex);
        }
        else
        {
            // otherwise, not string then it must be int
            SceneManager.LoadScene((int)(object)levelIndex);
        }
    }
}
