using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    //[SerializeField]private Button serverBtn;
    [SerializeField]private Button hostBtn;
    [SerializeField]private Button clientBtn;
    public GameObject NetworkManagerPrefab;
    public GameObject Player1Prefab;
    public GameObject Player2Prefab;

    private void Awake() 
    {
        if (GameObject.Find("NetworkManager(Clone)") == null) { Instantiate(NetworkManagerPrefab); }
        hostBtn.onClick.AddListener(() => {
            Debug.Log("Host connecting");
            //RemovePlayerControls();
            GameManager.instance.SetNetworkedScreen(true);
            PlayerManager.instance.isHost = true;
            PlayerManager.instance.currPlayer = 1;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.OnClientConnectedCallback += SetupTempNetworking;
            //PlayerManager.instance.currPlayerObject = GameObject.Find("Player 1");
        });

        clientBtn.onClick.AddListener(() => {
            Debug.Log("Client connecting");
            //RemovePlayerControls();
            GameManager.instance.SetNetworkedScreen(true);
            PlayerManager.instance.isHost = false;
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.OnClientConnectedCallback += SetupTempNetworking;
            //PlayerManager.instance.currPlayerObject = GameObject.Find("Player 2");
        });
    }


    private void SetupTempNetworking(ulong clientId)
    {
        print("SetupTempNetworking");
        //foreach (GameObject cloneObject in GameObject.FindGameObjectsWithTag("clone")) { Destroy(cloneObject); }


        //GameManager.instance.SetUpLevel();
        //GameObject currPlayer = PlayerManager.instance.currPlayerObject;
        //Destroy(currPlayer.GetComponent<MovementWASD>());
        //Destroy(currPlayer.GetComponent<MovementArrows>());
        //Destroy(currPlayer.GetComponent<PlayerMovement>());
        //PlayerMovement pm = currPlayer.AddComponent<PlayerMovement>();
        //pm.dashParticle = FindParticleSystem("DashParticles", currPlayer);
        //pm.momentumOutParticle = FindParticleSystem("MomentumTransferParticles", currPlayer);
        //pm.momentumInParticle = FindParticleSystem("MomentumRecieveParticle", currPlayer);
        //if (currPlayer.name == "Player 1") 
        //{ 
        //    pm.jumpParticle = FindParticleSystem("JumpPartRed", currPlayer);
        //    pm.wallJumpParticle = FindParticleSystem("SlideParticleParent/WallJumpRed", currPlayer);
        //    pm.slideParticle = FindParticleSystem("SlideParticleParent/SlideRed", currPlayer);
        //    pm.world = 1;
        //} else
        //{
        //    pm.jumpParticle = FindParticleSystem("JumpPartPurp", currPlayer);
        //    pm.wallJumpParticle = FindParticleSystem("SlideParticleParent/WallJumpPurp", currPlayer);
        //    pm.slideParticle = FindParticleSystem("SlideParticleParent/SlidePurp", currPlayer);
        //    pm.world = 2;
        //}
        GameManager.instance.GameEnable();
        SceneManager.sceneLoaded -= GameManager.instance.SetUpLevel;
        LevelLoader.instance.ReloadLevel();
    }

    private ParticleSystem FindParticleSystem(string systemName, GameObject playerObject)
    {
        Transform systemTransform = playerObject.transform.Find(systemName);
        if (systemTransform != null)
        {
            return systemTransform.GetComponent<ParticleSystem>();
        }
        else
        {
            Debug.LogError("ParticleSystem with name " + systemName + " not found on Player.");
            return null;
        }
    }

    private void RemovePlayerControls()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Destroy(player.GetComponent<MovementWASD>());
            Destroy(player.GetComponent<MovementArrows>());
            Destroy(player.GetComponent<PlayerMovement>());
        }
    }

}
