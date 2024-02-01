using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool networkingOn = false;
    public static GameManager instance;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    public GameObject shadowPrefab;

    private GameObject shadow1;
    private GameObject shadow2;


    private int currentScene;

    private void Awake()
    {
        GetPlayers();

        if (instance != null)
        {
            //Debug.LogError("Found more than one Game Manager in the scene.");
            Destroy(this.gameObject);
        }

        instance = this;

        MakeShadows();

        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        LoadNextLevel();
    }

    void Update()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene != scene)
        {
            print("new scene");
            currentScene = scene;
            LoadNextLevel();
            MakeShadows();
        }

        CopyAndSendPlayerInfo();
    }

    public void updateFromNetworkVariables(GameObject player1, GameObject player2, GameObject shadow1, GameObject shadow2) {
        this.player1 = player1;
        this.player2 = player2;
        this.shadow1 = shadow1;
        this.shadow2 = shadow2;
    }

    private void GetPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            PlayerMovement player = p.GetComponent<PlayerMovement>();
            if (player.world1)
            {
                player1 = p;
            } else
            {
                player2 = p;
            }
        }
    }

    private void MakeShadows()
    {
        shadow1 = Instantiate(shadowPrefab);
        shadow2 = Instantiate(shadowPrefab);
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

    public void SendMomentum(Vector2 momentum, GameObject sender)
    {
        if (sender == player1)
        {
            //print("sending to player2");
            MovementArrows playerMovement = player2.gameObject.GetComponent<MovementArrows>();
            playerMovement.QuantumLockAddMomentum(momentum);
        }
        else
        {
            //print("sending to player1");
            MovementWASD playerMovement = player1.gameObject.GetComponent<MovementWASD>();
            playerMovement.QuantumLockAddMomentum(momentum);
        }
    }

    public void LoadNextLevel()
    {
        GetPlayers();
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
        // rn the position is done by just making the shadow under the prefab
        if (player1 != null) {
            shadow1.transform.position = player1.transform.position + new Vector3(32, 0, 0);
            SpriteRenderer one = shadow1.GetComponent<SpriteRenderer>();
            one.sprite = player1.GetComponent<SpriteRenderer>().sprite;
        }
        if (player2 != null) {
            shadow2.transform.position = player2.transform.position + new Vector3(-32, 0, 0);
            SpriteRenderer two = shadow2.GetComponent<SpriteRenderer>();
            two.sprite = player2.GetComponent<SpriteRenderer>().sprite;
        }
    }

}
