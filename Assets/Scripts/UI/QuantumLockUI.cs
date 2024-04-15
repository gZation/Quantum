using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumLockUI : MonoBehaviour
{
    [SerializeField] private BranchLightning quantumLightning;


    // Start is called before the first frame updatde
    void Start()
    {
        PlayerManager.instance.OnVariableQLockChange += DisplayQuantumLockUI;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DisplayQuantumLockUI(PlayerManager.instance.qlocked);
    }
    void DisplayQuantumLockUI(bool newVal)
    {
        quantumLightning.SetEnabled(newVal);
    }
}
