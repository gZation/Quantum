using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance { get; private set; }

    private AudioSource edoTrackSource, cyberTrackSource, mainMenuSource;
    public List<AudioSource> sfxSources;
    public bool mainMenu = false;
    public bool cyberActive = false;
    public float masterVolume;
    public float sfxVolume;
    private float edoVolume, cyberVolume, mmVolume;
    public float crossfadeSpeed = 0.8f;

    private Dictionary<string, AudioSource> soundfx;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        soundfx = new Dictionary<string, AudioSource>();
        foreach (AudioSource sfx in sfxSources)
        {
            soundfx[sfx.name] = sfx;
        }
    }

    // Start is called before the first frame update
    void Start() {

        instance.edoTrackSource = transform.GetChild(0).GetComponent<AudioSource>();
        instance.cyberTrackSource = transform.GetChild(1).GetComponent<AudioSource>();
        instance.mainMenuSource = transform.GetChild(2).GetComponent<AudioSource>();

        if (!instance.mainMenu)
        {
            instance.edoVolume = !cyberActive ? 1.0f : 0.0f;
            instance.cyberVolume = cyberActive ? 1.0f : 0.0f;
        } else
        {
            instance.edoVolume = 0;
            instance.cyberVolume = 0;
            instance.mmVolume = 1;

            instance.Pause();
        }

    } // Start

    // Update is called once per frame
    void Update() {

        if (instance && !instance.mainMenu)
        {
            if (instance.cyberActive && cyberVolume < 1.0f)
            {
                instance.cyberVolume = Mathf.Clamp(instance.cyberVolume + crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
                instance.edoVolume = Mathf.Clamp(instance.edoVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
                instance.mmVolume = Mathf.Clamp(instance.mmVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
            }
            else if (!instance.cyberActive && instance.edoVolume < 1.0f)
            {
                instance.edoVolume = Mathf.Clamp(instance.edoVolume + crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
                instance.cyberVolume = Mathf.Clamp(instance.cyberVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
                instance.mmVolume = Mathf.Clamp(instance.mmVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);

            } // if
        } else
        {
            instance.edoVolume = Mathf.Clamp(instance.edoVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
            instance.cyberVolume = Mathf.Clamp(instance.cyberVolume - crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
            instance.mmVolume = Mathf.Clamp(instance.mmVolume + crossfadeSpeed * Time.deltaTime, 0.0f, 1.0f);
        }

        instance.mainMenuSource.volume = instance.masterVolume * instance.mmVolume;
        instance.edoTrackSource.volume = instance.masterVolume * instance.edoVolume;
        instance.cyberTrackSource.volume = instance.masterVolume * instance.cyberVolume;

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

    public void StartLevelMusic()
    {   
        if (PlayerManager.instance != null)
        {
            instance.mainMenu = false;
            
            if (GameManager.instance.IsNetworked())
            {
                instance.cyberActive = PlayerManager.instance.currPlayer == 2;
            } else
            {
                instance.cyberActive = PlayerManager.instance.playerOnLeft == 2;
            }
        }

        instance.UnPause();
    }

    public void Play(string sfx)
    {
        soundfx[sfx].Play();
    }

    public void Stop(string sfx)
    {
        soundfx[sfx].Stop();
    }

} // MusicManager
