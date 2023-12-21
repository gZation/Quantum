using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public GameObject player1;
    public GameObject player2;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Manager in the scene.");
        }

        instance = this;
    }

    public void QuantumLockPlayer(GameObject listener) 
    {
        //refactor later to be PlayerMovement once networking is in
        if (listener == player1)
        {
            print("Locking to player 2");
            MovementArrows playerMovement = player2.GetComponent<MovementArrows>();
            playerMovement.sharingMomentum = !playerMovement.sharingMomentum;
        } else
        {
            print("Locking to player 1");
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
}
