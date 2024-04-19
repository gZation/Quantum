using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using static PlayerSettings;
using UnityEngine.InputSystem;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance { get; private set; }
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private PlayerController[] playerControllers;
    
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

    public delegate void OnVariableChangeDelegate(bool newVal);
    public event OnVariableChangeDelegate OnVariableQLockChange;
    private bool m_qlocked = false;
    public bool qlocked
    {
        get { return m_qlocked; }
        set
        {
            if (m_qlocked == value) return;
            m_qlocked = value;
            if (OnVariableQLockChange != null) OnVariableQLockChange(m_qlocked);
        }
    }

    //Split screen
    public int playerOnLeft = 1;


    // Start is called before the first frame update
    void Awake()
    {
        playerControllers = new PlayerController[2];
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
        //print($"Players Length: ${players.Length}");
        if (players.Length == 0) return false;

        foreach (GameObject p in players)
        {
            PlayerSettings playerSetting = p.GetComponent<PlayerSettings>();
            if (playerSetting.world1)
            {
                player1 = p;

                if (playerControllers[0] != null)
                {
                    playerControllers[0].PlayerReference = p;
                    playerControllers[0].PlayerMovementRef = p.GetComponent<MovementWASD>();
                    foreach (PlayerMovement pm in playerControllers[0].PlayerReference.GetComponents<PlayerMovement>())
                    {
                        pm.DisableLegacyInput();
                    }
                        
                }
            }
            else
            {
                player2 = p;

                if (playerControllers[1] != null)
                {
                    playerControllers[1].PlayerReference = p;
                    playerControllers[1].PlayerMovementRef = p.GetComponent<MovementArrows>();
                    foreach (PlayerMovement pm in playerControllers[1].PlayerReference.GetComponents<PlayerMovement>())
                    {
                        pm.DisableLegacyInput();
                    }     
                }
                    
            }
            //Debug.Log("Play Reference Changed!!");
        }
        MakeShadows();
        if (GameManager.instance.IsNetworked()) { return setNetworkedPlayers(); }
        else return true;
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

    public void ToggleQuantumLock()
    {
        if (!GameManager.instance.IsNetworked())
        { 
            instance.qlocked = !instance.qlocked;
        } else
        {
            UpdateQLockServerRpc();
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void UpdateQLockServerRpc()
    {
        UpdateQLockClientRpc();
    }

    [ClientRpc]
    public void UpdateQLockClientRpc()
    {
        instance.qlocked = !instance.qlocked;
    }



    public void SendMomentum(Vector2 momentum, GameObject sender)
    {
        if (!GameManager.instance.IsNetworked())
        {
            if (instance.qlocked) {
                if (sender == player1)
                {
                    player2.GetComponent<PlayerMovement>().QuantumLockAddMomentum(momentum);
                }
                else if (sender == player2 )
                {
                    player1.gameObject.GetComponent<PlayerMovement>().QuantumLockAddMomentum(momentum);
                }

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
    public void UpdateMomentumClientRpc(Vector2 momentum) { updateMomentum(momentum); }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMomentumServerRpc(Vector2 momentum) { updateMomentum(momentum); }

    public void updateMomentum(Vector2 momentum)
    {
        if (instance.qlocked)
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
            SpriteRenderer player = player1.GetComponentInChildren<SpriteRenderer>();
            SpriteRenderer shadow = shadow1.GetComponentInChildren<SpriteRenderer>();
            shadow.sprite = player.sprite;
            shadow.flipX = player.flipX;
        }
        if (player2 != null)
        {
            shadow2.transform.position = player2.transform.position + new Vector3(-32, 0, 0);
            SpriteRenderer player = player2.GetComponentInChildren<SpriteRenderer>();
            SpriteRenderer shadow = shadow2.GetComponentInChildren<SpriteRenderer>();
            shadow.sprite = player.sprite;
            shadow.flipX = player.flipX;
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


    public void HandlePlayerControllerEnter(PlayerInput pi)
    {
        for (int i = 0; i < playerControllers.Length; i++)
        {
            if (playerControllers[i] == null)
            {
                playerControllers[i] = pi.GetComponent<PlayerController>();
                playerControllers[i].PlayerReference = i == 0 ? player1 : player2;
                playerControllers[i].PlayerReference.GetComponent<PlayerMovement>().DisableLegacyInput();
                break;
            } 
        }   
    }


    public void HandlePlayerControllerExit(PlayerInput pi)
    {

    }

}
