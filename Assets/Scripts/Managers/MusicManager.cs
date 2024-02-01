using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance { get; private set; }

    private AudioSource edoTrackSource, cyberTrackSource;
    public bool cyberActive = false;
    public float masterVolume;
    private float edoVolume, cyberVolume;
    public float crossfadeSpeed = 1.0f;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogError("Found more than one Music Manager in the scene.");
            Destroy(this.gameObject);
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start() {

        edoTrackSource = this.transform.GetChild(0).GetComponent<AudioSource>();
        cyberTrackSource = this.transform.GetChild(1).GetComponent<AudioSource>();

        masterVolume = 1.0f;
        edoVolume = !cyberActive ? 1.0f : 0.0f;
        cyberVolume = cyberActive ? 1.0f : 0.0f;

    } // Start

    // Update is called once per frame
    void Update() {
        
        if (cyberActive && cyberVolume < 1.0f) {
            cyberVolume = Mathf.Clamp(cyberVolume + crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
            edoVolume = Mathf.Clamp(edoVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);

        } else if (!cyberActive && edoVolume < 1.0f) {
            edoVolume = Mathf.Clamp(edoVolume + crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
            cyberVolume = Mathf.Clamp(cyberVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);

        } // if

        edoTrackSource.volume = masterVolume * edoVolume;
        cyberTrackSource.volume = masterVolume * cyberVolume;

    } // Update

    public void Pause() {
        edoTrackSource.Pause();
        cyberTrackSource.Pause();

    } // Pause

    public void UnPause() {
        edoTrackSource.UnPause();
        cyberTrackSource.UnPause();

    } // UnPause

} // MusicManager
