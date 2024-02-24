using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSelection {
    NEUTRAL = 1,
    BOY = 0,
    GIRL = 2

} // CharacterSelection

public class BackgroundManager : MonoBehaviour {

    private RectTransform edoMaskRect, cyberMaskRect;
    private RectTransform edoBGRect, cyberBGRect;

    [SerializeField] private float xScaleTarget;
    private float lerpProgress;
    [SerializeField] private int lerpSpeed = 3;
    [SerializeField] private CharacterSelection selection;


    // Start is called before the first frame update
    void Start() {
        
        selection = CharacterSelection.NEUTRAL;
        xScaleTarget = 0.5f;
        lerpProgress = 1.0f;

        edoMaskRect = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        edoBGRect = transform.GetChild(0).GetChild(0).gameObject.GetComponent<RectTransform>();
        // cyberMaskRect = transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        // cyberBGRect = transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();

        edoMaskRect.sizeDelta = new Vector2(Screen.width / 2, Screen.height);
        edoBGRect.localScale = new Vector3(1, 1, 0);

    } // Start

    // Update is called once per frame
    void Update() {
        
        if (Input.GetButtonDown("Horizontal")) {

            if (Input.GetAxisRaw("Horizontal") > 0) {
                selection = CharacterSelection.GIRL;
                lerpProgress = 0f;
            
            } else {
                selection = CharacterSelection.BOY;
                lerpProgress = 0f;

            } // if

        } // if
        

        xScaleTarget = Mathf.Clamp((((int) selection) / 2.0f) - 0.5f, -0.5f, 0.5f);
        lerpProgress = Mathf.Clamp(((int)(lerpProgress * 100) + lerpSpeed) / 100.0f, 0, 1);

        edoMaskRect.sizeDelta = new Vector2(
            Mathf.Lerp(edoMaskRect.sizeDelta.x, Screen.width * (1 + xScaleTarget) / 2, lerpProgress), 
            Screen.height
        );
        edoBGRect.localScale = new Vector3(1, 1, 0);

    } // Update

} // BackgroundManager