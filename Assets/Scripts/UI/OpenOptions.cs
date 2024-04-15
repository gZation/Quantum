using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class OpenOptions : MonoBehaviour
{

    bool happened;
    float timeWait;
    float timePassed;
    bool done;

    private void Start()
    {
        timePassed = 0;
        timeWait = 1f;
        happened = true;
        done = false;
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > timeWait && !done)
        {
            happened = false;
            done = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && !happened)
        {
            PauseMenu.instance.TriggerPause();
            PauseMenu.instance.ForceOpenControls();
            happened = true;
        }
    }
}
