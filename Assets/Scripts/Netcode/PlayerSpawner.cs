using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour {
    [SerializeField] private GameObject playerPrefab1;
    [SerializeField] private GameObject playerShadowPrefab1;

    [SerializeField] private GameObject playerPrefab2;
    [SerializeField] private GameObject playerShadowPrefab2;

    [SerializeField] private GameObject player1Spawn;
    [SerializeField] private GameObject player2Spawn;

    public NetworkVariable<NetworkObjectReference> player1 = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<NetworkObjectReference> player2 = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<NetworkObjectReference> shadow1 = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<NetworkObjectReference> shadow2 = new NetworkVariable<NetworkObjectReference>();


    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ushort clientId) {

        GameObject newPlayer;
        GameObject newShadow;
        if (clientId == 0) {
            newPlayer = Instantiate(playerPrefab1, player1Spawn.transform.position, Quaternion.identity);
            newShadow = Instantiate(playerShadowPrefab1, player1Spawn.transform.position - new Vector3(32, 0, 0), Quaternion.identity);
            spawnPlayerAndShadow(newPlayer, newShadow, clientId);
            GameManager.instance.player1 = newPlayer;
            GameManager.instance.shadow1 = newShadow;
        } else {
            newPlayer = Instantiate(playerPrefab2, player2Spawn.transform.position, player2Spawn.transform.rotation);
            newShadow = Instantiate(playerShadowPrefab2, player2Spawn.transform.position - new Vector3(-32, 0, 0), player2Spawn.transform.rotation);
            spawnPlayerAndShadow(newPlayer, newShadow, clientId);
            GameManager.instance.player2 = newPlayer;
            GameManager.instance.shadow2 = newShadow;
        }


        if (GameManager.instance.player1 && GameManager.instance.player2)
            ClientConnectedClientRPC(GameManager.instance.player1, GameManager.instance.player2, GameManager.instance.shadow1, GameManager.instance.shadow2);
    }

    private void spawnPlayerAndShadow(GameObject player, GameObject shadow, ushort clientId) {
        
        NetworkObject playerNetObj = player.GetComponent<NetworkObject>();
        player.SetActive(true);
        playerNetObj.SpawnAsPlayerObject(clientId, true);

        NetworkObject shadowNetObj = shadow.GetComponent<NetworkObject>();
        shadowNetObj.Spawn();
        
    }

    [ClientRpc]
    private void ClientConnectedClientRPC(NetworkObjectReference player1, NetworkObjectReference player2, NetworkObjectReference shadow1, NetworkObjectReference shadow2) {
        GameManager.instance.player1 = player1;
        GameManager.instance.player2 = player2;
        GameManager.instance.shadow1 = shadow1;
        GameManager.instance.shadow2 = shadow2;
        Instantiate(playerShadowPrefab1, player1Spawn.transform.position - new Vector3(32, 0, 0), player1Spawn.transform.rotation);
        Instantiate(playerShadowPrefab2, player2Spawn.transform.position - new Vector3(-32, 0, 0), player2Spawn.transform.rotation);
        
    }


    public override void OnNetworkSpawn() {
        if (IsHost)
            SpawnPlayerServerRpc(0);
        else {
            SpawnPlayerServerRpc(1);
        }
            


    }
}
