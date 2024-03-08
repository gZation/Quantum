using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumLockUI : MonoBehaviour
{
    [SerializeField] private GameObject QuantumLockUIText;
    [SerializeField] private GameObject player;


    // Start is called before the first frame updatde
    void Start()
    {
        QuantumLockUIText.SetActive(false);
        player.GetComponent<PlayerSettings>().OnVariableQLockChange += DisplayQuantumLockUI;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DisplayQuantumLockUI(player.GetComponent<PlayerSettings>().qlocked);
    }
    void DisplayQuantumLockUI(bool newVal)
    {
        QuantumLockUIText.SetActive(newVal);
    }
}
