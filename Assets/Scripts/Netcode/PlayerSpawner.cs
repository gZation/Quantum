using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour {
    [SerializeField] private GameObject playerPrefab1;
    [SerializeField] private GameObject playerPrefab2;
    [SerializeField] private GameObject playerShadowPrefab;

    public NetworkVariable<NetworkObjectReference> player1 = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<NetworkObjectReference> player2 = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<NetworkObjectReference> shadow1 = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<NetworkObjectReference> shadow2 = new NetworkVariable<NetworkObjectReference>();


    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ushort clientId) {
        GameObject newPlayer;
        GameObject newShadow;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        Vector3 player1Location = Vector3.zero;
        Vector3 player2Location = Vector3.zero;

        foreach (GameObject p in players)
        {
            PlayerMovement player = p.GetComponent<PlayerMovement>();
            if (player.world1)
            {
                player1Location = p.transform.position;
            }
            else
            {
                player2Location = p.transform.position;
            }
        }

        if (clientId == 1) { // If the player is 1
            newPlayer = Instantiate(playerPrefab1, player1Location, Quaternion.identity);
            newShadow = Instantiate(playerShadowPrefab, player1Location - new Vector3(32, 0, 0), Quaternion.identity);
            spawnPlayerAndShadow(newPlayer, newShadow, clientId);
            GameManager.instance.SetPlayerAndShadow(newPlayer, newShadow, 1);
        } else { // if player is 2
            newPlayer = Instantiate(playerPrefab2, player2Location, Quaternion.identity);
            newShadow = Instantiate(playerShadowPrefab, player2Location - new Vector3(-32, 0, 0), Quaternion.identity);
            spawnPlayerAndShadow(newPlayer, newShadow, clientId);
            GameManager.instance.SetPlayerAndShadow(newPlayer, newShadow, 2);
        }


        if (GameManager.instance.GetPlayer(1) && GameManager.instance.GetPlayer(2))
            ClientConnectedClientRPC(GameManager.instance.GetPlayer(1), GameManager.instance.GetPlayer(2), 
                GameManager.instance.GetShadow(1), GameManager.instance.GetShadow(2));
    }

    // black magic 30% chase doesn't understand
    private void spawnPlayerAndShadow(GameObject player, GameObject shadow, ushort clientId) {
        
        NetworkObject playerNetObj = player.GetComponent<NetworkObject>();
        player.SetActive(true);
        playerNetObj.SpawnAsPlayerObject(clientId, true);

        NetworkObject shadowNetObj = shadow.GetComponent<NetworkObject>();
        shadowNetObj.Spawn();
        
    }

    [ClientRpc]
    private void ClientConnectedClientRPC(NetworkObjectReference player1, NetworkObjectReference player2, NetworkObjectReference shadow1, NetworkObjectReference shadow2) {
        GameManager.instance.SetPlayerAndShadow(player1, shadow1, 1);
        GameManager.instance.SetPlayerAndShadow(player2, shadow2, 1);

        GameManager.instance.MakeShadows();
    }


    // Whenever an item is spawned, put on network 
    // oh no
    // oh no
    // someone enters as client or host
    // TODO: chase fix this tut, only remake players and shadows if new room
    public override void OnNetworkSpawn() {
        if (IsHost)
            SpawnPlayerServerRpc(1);
        else {
            SpawnPlayerServerRpc(2);
        }
    }
}
