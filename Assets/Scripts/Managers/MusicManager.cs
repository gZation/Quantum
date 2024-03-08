using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance { get; private set; }

    private AudioSource edoTrackSource, cyberTrackSource;
    public bool cyberActive = false;
    public float masterVolume;
    public float sfxVolume;
    private float edoVolume, cyberVolume;
    public float crossfadeSpeed = 1.0f;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogError("Found more than one Music Manager in the scene.");
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start() {

        instance.edoTrackSource = transform.GetChild(0).GetComponent<AudioSource>();
        instance.cyberTrackSource = transform.GetChild(1).GetComponent<AudioSource>();

        instance.edoVolume = !cyberActive ? 1.0f : 0.0f;
        instance.cyberVolume = cyberActive ? 1.0f : 0.0f;

    } // Start

    // Update is called once per frame
    void Update() {
        
        if (cyberActive && cyberVolume < 1.0f) {
            instance.cyberVolume = Mathf.Clamp(cyberVolume + crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
            instance.edoVolume = Mathf.Clamp(edoVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);

        } else if (!cyberActive && edoVolume < 1.0f) {
            instance.edoVolume = Mathf.Clamp(edoVolume + crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
            instance.cyberVolume = Mathf.Clamp(cyberVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);

        } // if

        instance.edoTrackSource.volume = masterVolume * edoVolume;
        instance.cyberTrackSource.volume = masterVolume * cyberVolume;

    } // Update

    public void Pause() {
        instance.edoTrackSource.Pause();
        instance.cyberTrackSource.Pause();

    } // Pause

    public void UnPause() {
        instance.edoTrackSource.UnPause();
        instance.cyberTrackSource.UnPause();

    } // UnPause

    public static void UpdateMasterVolume(float value)
    {
        instance.masterVolume = value;
    }

    public static void UpdateSFXVolume(float  value)
    {
        instance.sfxVolume = value;
    }

} // MusicManager
