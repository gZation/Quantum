using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingPlatforms : MonoBehaviour
{
    public GameObject platformPosition1;
    public GameObject platformPosition2;

    public bool resetOnStart;

    [SerializeField] private bool one;

    private void Start()
    {
        if (resetOnStart)
        {
            platformPosition1.SetActive(true);
            platformPosition2.SetActive(false);
            one = true;

            resetOnStart = false;
        }
    }

    public void ChangePlatforms()
    {
        one = !one;
        platformPosition1.SetActive(one);
        platformPosition2.SetActive(!one);

        GameManager.instance.CopyAndSendWorldInfo();
    }
}
