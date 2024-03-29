using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumLockUI : MonoBehaviour
{
    [SerializeField] private GameObject QuantumLockUIText;


    // Start is called before the first frame updatde
    void Start()
    {
        QuantumLockUIText.SetActive(false);
        PlayerManager.instance.OnVariableQLockChange += DisplayQuantumLockUI;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DisplayQuantumLockUI(PlayerManager.instance.qlocked);
    }
    void DisplayQuantumLockUI(bool newVal)
    {
        QuantumLockUIText.SetActive(newVal);
    }
}
