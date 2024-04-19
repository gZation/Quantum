using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class OpenOptions : MonoBehaviour
{
    float timeWait;
    float timePassed;
    bool done;

    private void Start()
    {
        timePassed = 0;
        timeWait = 1.2f;
        done = false;

        if (PlayerManager.instance.hostPlayer.Value != PlayerManager.instance.currPlayer)
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > timeWait && !done)
        {
            done = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && done)
        {
            PauseMenu.instance.TriggerPause();
            PauseMenu.instance.ForceOpenControls();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
