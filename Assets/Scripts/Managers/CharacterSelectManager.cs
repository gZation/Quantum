using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// public enum CharacterSelection {
//     NEUTRAL,
//     BOY,
//     GIRL

// } // Character

public class CharacterSelectManager : MonoBehaviour {
    
    GameObject p1, p2;
    RectTransform p1Rect, p2Rect;
    float p1Lerp, p2Lerp;
    float p1Target, p2Target;
    Vector3 boyPosition, girlPosition;

    // Start is called before the first frame update
    void Start() {

        p1 = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        p2 = GameObject.Find("Canvas").transform.GetChild(2).gameObject;

        p1Rect = p1.GetComponent<RectTransform>();
        p2Rect = p2.GetComponent<RectTransform>();

        p1Lerp = 0.5f;
        p1Target = 0.5f;
        p2Lerp = 0.5f;
        p2Target = 0.5f;

        boyPosition = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<RectTransform>().localPosition;
        girlPosition = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<RectTransform>().localPosition;

    } // Start

    // Update is called once per frame
    void Update() {
        
        if (Input.GetButtonDown("Horizontal")) {

            if (Input.GetAxisRaw("Horizontal") > 0) {
                p1Target = Mathf.Clamp(p1Target + 0.5f, 0, 1);
            
            } else {
                p1Target = Mathf.Clamp(p1Target - 0.5f, 0, 1);

            } // if

        } // if

        if (p1Lerp < p1Target) p1Lerp = (Mathf.RoundToInt(p1Lerp * 10) + 1) / 10.0f;
        if (p1Lerp > p1Target) p1Lerp = (Mathf.RoundToInt(p1Lerp * 10) - 1) / 10.0f;

        if (p2Lerp < p2Target) p2Lerp = (Mathf.RoundToInt(p2Lerp * 10) + 1) / 10.0f;
        if (p2Lerp > p2Target) p2Lerp = (Mathf.RoundToInt(p2Lerp * 10) - 1) / 10.0f;

        p1Rect.localPosition = Vector3.LerpUnclamped(
            new Vector3(boyPosition.x, p1Rect.localPosition.y, 0), 
            new Vector3(girlPosition.x, p1Rect.localPosition.y, 0), 
            p1Lerp
        );

        p2Rect.localPosition = Vector3.LerpUnclamped(
            new Vector3(boyPosition.x, p2Rect.localPosition.y, 0), 
            new Vector3(girlPosition.x, p2Rect.localPosition.y, 0), 
            p2Lerp
        );

    } // Update
} // CharacterSelectManager