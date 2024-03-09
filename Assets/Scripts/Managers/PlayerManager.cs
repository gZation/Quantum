using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance { get; private set; }
    private GameObject player1;
    private GameObject player2;
    private GameObject shadow1;
    private GameObject shadow2;
    public bool isHost;

    // Currently the curr player object needs to be set for networking. In the future, we might need to change this to a int/string since the player might not be instantiated yet in the menu screen.
    // Right now, this object is set by the NetworkManagerUI
    // Change would require update to SetNetworkedPlayers()

    // currPlayer = 1 if player1, 2 if player2
    public NetworkVariable<int> hostPlayer = new NetworkVariable<int>();
    public int currPlayer;

    public GameObject currPlayerObject;
    public GameObject otherPlayerObject;


    [SerializeField] protected GameObject shadowPrefab;

    //Split screen
    public int playerOnLeft = 1;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            //Debug.LogError("Found more than one Player Manager in the scene.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;

    }

    public void GameEnable()
    {
        //if (networkingOn)
        //{
        //    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetUpLevel;
        //} else
        //{
        //    SceneManager.sceneLoaded += SetUpLevel;
        //}
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isGameEnabled)
        {
            return;
        }
        CopyAndSendPlayerInfo();
    }

    //Set up the players. If the players don't exist yet, then return false. Otherwise, return true
    public bool SetPlayersAndShadows()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return false;

        foreach (GameObject p in players)
        {
            PlayerSettings playerSetting = p.GetComponent<PlayerSettings>();
            if (playerSetting.world1)
            {
                player1 = p;
            }
            else
            {
                player2 = p;
            }
        }
        MakeShadows();
        if (GameManager.instance.IsNetworked()) { return setNetworkedPlayers(); }
        else return false;
    }

    private bool setNetworkedPlayers()
    {
        if (!IsHost)
        {
            if (hostPlayer.Value == 0) return false;
            currPlayer = hostPlayer.Value == 1 ? 2 : 1;
        }
        else
        {
            hostPlayer.Value = currPlayer;
        }
        currPlayerObject = (currPlayer == 1)  ? player1 : player2;


        otherPlayerObject = currPlayerObject == player1 ? player2 : player1;
        PlayerSettings playerSetting = currPlayerObject.GetComponent<PlayerSettings>();
        playerSetting.isActivePlayer = true;

        // If the current user is the host, setup host specific stuff like variables
        if (IsHost)
        {
            AssignOwnership();
        } else
        // If the current user is the client, setup client specific stuff like variables
        {
        }
        return true;
    }

    private void AssignOwnership()
    {
        otherPlayerObject.GetComponent<NetworkObject>().ChangeOwnership(1);
        //Debug.Log("Owndership assigned");
        //Debug.Log(otherPlayerObject.GetComponent<NetworkObject>().OwnerClientId);
        //this.GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
        {
        }
    }

    public void MakeShadows()
    {
        shadow1 = Instantiate(shadowPrefab);
        shadow2 = Instantiate(shadowPrefab);
    }
 

    public bool isPlayersInit()
    {
        return player1 != null && player2 != null;
    }

    public void QuantumLockPlayer(GameObject listener)
    {
        if (!GameManager.instance.IsNetworked())
        {
            if (listener == player1)
            {
                MovementArrows playerMovement = player2.GetComponent<MovementArrows>();
                playerMovement.sharingMomentum = !playerMovement.sharingMomentum;
            }
            else
            {
                MovementWASD playerMovement = player1.GetComponent<MovementWASD>();
                playerMovement.sharingMomentum = !playerMovement.sharingMomentum;
            }
        } else
        {

        }

    }

    public void SendMomentum(Vector2 momentum, GameObject sender)
    {
        if (!GameManager.instance.IsNetworked())
        {
            if (sender == player1 && player2.GetComponent<PlayerSettings>().qlocked)
            {
                player2.GetComponent<MovementArrows>().QuantumLockAddMomentum(momentum);
            }
            else if (sender == player2 && player1.GetComponent<PlayerSettings>().qlocked)
            {
                player1.gameObject.GetComponent<MovementWASD>().QuantumLockAddMomentum(momentum);
            }
        } 
        else
        {
            if (NetworkManager.Singleton.IsHost)
            {
                UpdateMomentumClientRpc(momentum);
            } else
            {
                UpdateMomentumServerRpc(momentum);
            }
        }
    }

    [ClientRpc]
    public void UpdateMomentumClientRpc(Vector2 momentum) { if (!NetworkManager.Singleton.IsHost) updateMomentum(momentum); }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMomentumServerRpc(Vector2 momentum) { updateMomentum(momentum); }

    public void updateMomentum(Vector2 momentum)
    {
        if (currPlayerObject.GetComponent<PlayerSettings>().qlocked)
        {
            currPlayerObject.GetComponent<PlayerMovement>().QuantumLockAddMomentum(momentum);
        }
    }

    public void CopyAndSendPlayerInfo()
    {
        // rn the position is done by just making the shadow under the prefab
        if (player1 != null)
        {
            shadow1.transform.position = player1.transform.position + new Vector3(32, 0, 0);
            SpriteRenderer one = shadow1.GetComponentInChildren<SpriteRenderer>();
            one.sprite = player1.GetComponentInChildren<SpriteRenderer>().sprite;
        }
        if (player2 != null)
        {
            shadow2.transform.position = player2.transform.position + new Vector3(-32, 0, 0);
            SpriteRenderer two = shadow2.GetComponentInChildren<SpriteRenderer>();
            two.sprite = player2.GetComponentInChildren<SpriteRenderer>().sprite;
        }
    }

    public GameObject GetPlayer(int num)
    {
        if (num == 1)
        {
            return player1;
        }
        else if (num == 2)
        {
            return player2;
        }

        return null;
    }

    public GameObject GetShadow(int num)
    {
        if (num == 1)
        {
            return shadow1;
        }
        else if (num == 2)
        {
            return shadow2;
        }

        return null;
    }

    //public void SetPlayerAndShadow(GameObject player, GameObject shadow, int num)
    //{
    //    if (num == 1)
    //    {
    //        player1 = player;
    //        shadow1 = shadow;
    //    }
    //    else if (num == 2)
    //    {
    //        player2 = player;
    //        shadow2 = shadow;
    //    }
    //}

}
