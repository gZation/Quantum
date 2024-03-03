using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSelection {
    NEUTRAL = 1,
    BOY = 0,
    GIRL = 2

} // CharacterSelection

public class SelectScreenManager : MonoBehaviour {

    private RectTransform edoMaskRect, cyberMaskRect;
    private RectTransform edoBGRect, cyberBGRect;
    private RectTransform boyRect, girlRect;
    private Image boyImage, girlImage;

    [SerializeField] private float xScaleTarget, imgScaleTarget;
    private int lerpProgress;
    [SerializeField] private int lerpSpeed = 3;
    [SerializeField] private CharacterSelection selection;


    // Start is called before the first frame update
    void Start() {
        
        selection = CharacterSelection.NEUTRAL;
        xScaleTarget = 0.5f;
        lerpProgress = 100;

        edoMaskRect = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        edoBGRect = transform.GetChild(0).GetChild(0).gameObject.GetComponent<RectTransform>();
        cyberMaskRect = transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        cyberBGRect = transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();

        boyRect = transform.GetChild(0).GetChild(1).gameObject.GetComponent<RectTransform>();
        boyImage = transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
        girlRect = transform.GetChild(1).GetChild(1).gameObject.GetComponent<RectTransform>();
        girlImage = transform.GetChild(1).GetChild(1).gameObject.GetComponent<Image>();

        edoMaskRect.sizeDelta = new Vector2(Screen.width / 2, Screen.height);
        edoBGRect.localScale = new Vector3(1, 1, 0);
        cyberMaskRect.sizeDelta = new Vector2(Screen.width / 2, Screen.height);
        cyberBGRect.localScale = new Vector3(1, 1, 0);

    } // Start

    // Update is called once per frame
    void Update() {
        
        if (Input.GetButtonDown("Horizontal")) {

            if (Input.GetAxisRaw("Horizontal") > 0) {
                selection = CharacterSelection.GIRL;
                lerpProgress = 0;
            
            } else {
                selection = CharacterSelection.BOY;
                lerpProgress = 0;

            } // if

        } // if

        if (Input.GetKeyDown(KeyCode.Space)) {
            GoToGame();
            
        } // if

        xScaleTarget = Mathf.Clamp((((int) selection) / 2.0f) - 0.5f, -0.5f, 0.5f);
        imgScaleTarget = ((int)(xScaleTarget * 10) + 1) / 100.0f;
        lerpProgress = Mathf.Clamp(lerpProgress + lerpSpeed, 0, 100);

        edoMaskRect.sizeDelta = new Vector2(
            Mathf.Lerp(edoMaskRect.sizeDelta.x, Screen.width * (1 - xScaleTarget) / 2, lerpProgress / 100.0f), 
            Screen.height
        );
        
        edoBGRect.localScale = new Vector3(
            Mathf.Lerp(edoBGRect.localScale.x, 1f - imgScaleTarget, lerpProgress / 100.0f), 
            Mathf.Lerp(edoBGRect.localScale.y, 1f - imgScaleTarget, lerpProgress / 100.0f), 
            0
        );

        cyberMaskRect.sizeDelta = new Vector2(
            Mathf.Lerp(cyberMaskRect.sizeDelta.x, Screen.width * (1 + xScaleTarget) / 2, lerpProgress / 100.0f), 
            Screen.height
        );

        cyberBGRect.localScale = new Vector3(
            Mathf.Lerp(cyberBGRect.localScale.x, 1f + imgScaleTarget, lerpProgress / 100.0f), 
            Mathf.Lerp(cyberBGRect.localScale.y, 1f + imgScaleTarget, lerpProgress / 100.0f),  
            0
        );
        // Lower opacity if 0 <= xScaleTarget <= 0.5
        boyImage.color = new Color(
            boyImage.color.r, boyImage.color.g, boyImage.color.b, 
            (selection == CharacterSelection.GIRL) ? Mathf.Lerp(1, 0.5f, lerpProgress / 100.0f) : Mathf.Lerp(0.5f, 1, lerpProgress / 100.0f)
        );
        // Lower opacity if -0.5 <= xScaleTarget <= 0
        girlImage.color = new Color(
            girlImage.color.r, girlImage.color.g, girlImage.color.b, 
            (selection == CharacterSelection.BOY) ? Mathf.Lerp(1, 0.5f, lerpProgress / 100.0f) : Mathf.Lerp(0.5f, 1, lerpProgress / 100.0f)
        );

    } // Update

    public void GoToGame() {

        switch (selection) {

            case CharacterSelection.BOY:
                Debug.Log("Go to game (Host is BOY)");
                break;

            case CharacterSelection.GIRL:
                Debug.Log("Go to game (Host is GIRL)");
                break;

            default:
                break;

        } // switch

    } // StartGame

} // BackgroundManager