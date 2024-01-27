using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour {
    [SerializeField] private GameObject playerPrefabA; //add prefab in inspector
    [SerializeField] private GameObject playerPrefabB; //add prefab in inspector

    public GameObject player1Spawn;
    public GameObject player2Spawn;


    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientId) {
        Debug.Log("ClientID: " + clientId);
        GameObject newPlayer;
        if (clientId == 0)
            newPlayer = Instantiate(playerPrefabA, player1Spawn.transform.position, player1Spawn.transform.rotation);
        else
            newPlayer = Instantiate(playerPrefabB, player2Spawn.transform.position, player2Spawn.transform.rotation);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
    }

    private void Start() {
        Debug.Log("Start");
    }

    private void Awake() {
        Debug.Log("Awake");
    }

    public override void OnNetworkSpawn() {
        Debug.Log("OnNetworkSpawned called");
        if (IsHost)
            SpawnPlayerServerRpc(0);
        else
            SpawnPlayerServerRpc(1);
    }
}
