using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumLockUI : MonoBehaviour
{
    [SerializeField] private GameObject QuantumLockUIText;
    [SerializeField] private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        QuantumLockUIText.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DisplayQuantumLockUI();
    }
    void DisplayQuantumLockUI()
    {
        if (player.GetComponent<PlayerSettings>().locked)
        {
            // display the quantum lock UI
            QuantumLockUIText.SetActive(true);
        } else {
            // display the quantum lock UI
            QuantumLockUIText.SetActive(false);
        }
    }
}
