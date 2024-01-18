using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public GameObject player1;
    public GameObject player2;
    private bool networked = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Manager in the scene.");
        }

        instance = this;
    }

    private void Start()
    {
        LoadNextLevel();
    }

    public void QuantumLockPlayer(GameObject listener) 
    {
        //refactor later to be PlayerMovement once networking is in
        if (listener == player1)
        {
            print("Toggling lock to player 2");
            MovementArrows playerMovement = player2.GetComponent<MovementArrows>();
            playerMovement.sharingMomentum = !playerMovement.sharingMomentum;
        } else
        {
            print("Toggling lock to player 1");
            MovementWASD playerMovement = player1.GetComponent<MovementWASD>();
            playerMovement.sharingMomentum = !playerMovement.sharingMomentum;
        }
    }

    public void SendMomentum(Vector2 velocity, GameObject sender)
    {
        if (sender == player1)
        {
            print("sending to player2");
            MovementArrows playerMovement = player2.gameObject.GetComponent<MovementArrows>();
            playerMovement.AddMomentum(velocity);
        }
        else
        {
            print("sending to player1");
            MovementWASD playerMovement = player1.gameObject.GetComponent<MovementWASD>();
            playerMovement.AddMomentum(velocity);
        }
    }

    public void LoadNextLevel()
    {
        CopyAndSendWorldInfo();
    }

    public void CopyAndSendWorldInfo()
    {
        GameObject[] world1Level = GameObject.FindGameObjectsWithTag("World1Level");
        
        foreach (GameObject go in world1Level)
        {
            GameObject transferVersion = Instantiate(go);
            transferVersion.layer = LayerMask.NameToLayer("World1");

            int children = transferVersion.transform.childCount;

            for(int i = 0; i < children; i++)
            {
                GameObject child = transferVersion.transform.GetChild(i).gameObject;
                child.layer = LayerMask.NameToLayer("World1");
            }

            transferVersion.transform.parent = go.transform.parent;
            transferVersion.transform.position = go.transform.position + new Vector3(32, 0, 0);

            Tilemap tilemap = transferVersion.GetComponentInChildren<Tilemap>();
            Color color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0.5f);
            tilemap.color = color;
        }

        GameObject[] world2Level = GameObject.FindGameObjectsWithTag("World2Level");

        foreach (GameObject go in world2Level)
        {
            GameObject transferVersion = Instantiate(go);
            transferVersion.layer = LayerMask.NameToLayer("World2");

            int children = transferVersion.transform.childCount;

            for (int i = 0; i < children; i++)
            {
                GameObject child = transferVersion.transform.GetChild(i).gameObject;
                child.layer = LayerMask.NameToLayer("World2");
            }

            transferVersion.transform.parent = go.transform.parent;
            transferVersion.transform.position = go.transform.position + new Vector3(-32, 0, 0);

            Tilemap tilemap = transferVersion.GetComponentInChildren<Tilemap>();
            Color color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0.5f);
            tilemap.color = color;
        }
    }

    public void CopyAndSendPlayerInfo()
    {

    }

    public void SetNetworked()
    {
        networked = true;
    }
}
