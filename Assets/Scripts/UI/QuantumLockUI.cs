using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumLockUI : MonoBehaviour
{
    [SerializeField] private GameObject QuantumLockUIText1;
    [SerializeField] private GameObject QuantumLockUIText2;


    // Start is called before the first frame update
    void Start()
    {
        QuantumLockUIText1.SetActive(false);
        QuantumLockUIText2.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DisplayQuantumLockUI();
    }
    void DisplayQuantumLockUI()
    {
        GameObject player1 = GameManager.instance.GetPlayer(1);
        GameObject player2 = GameManager.instance.GetPlayer(2);
        // getPlayer1, if player1 is sharingMomemtum, they are quantum locked
        if (player1.GetComponent<PlayerMovement>().sharingMomentum)
        {
            // display the quantum lock UI
            QuantumLockUIText2.SetActive(true);
        } else {
            QuantumLockUIText2.SetActive(false);
        }
        if (player2.GetComponent<PlayerMovement>().sharingMomentum)
        {
            // display the quantum lock UI
            QuantumLockUIText1.SetActive(true);
        } else {
            // display the quantum lock UI
            QuantumLockUIText1.SetActive(false);
        }
    }
}
