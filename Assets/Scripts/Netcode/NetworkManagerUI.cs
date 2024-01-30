using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    //[SerializeField]private Button serverBtn;
    [SerializeField]private Button hostBtn;
    [SerializeField]private Button clientBtn;


    private void Awake() 
    {
        //serverBtn.onClick.AddListener(() => {
        //    NetworkManager.Singleton.StartServer();
        //});

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() => {
            //Debug.Log("Connecting Client");
            NetworkManager.Singleton.StartClient();
        });
    }
}
