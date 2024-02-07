using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public NetworkManager networkManager;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [SerializeField] private bool networkingOn = false;
    public bool startFromScene = true;

    public GameObject shadowPrefab;

    [SerializeField] private GameObject shadow1;
    [SerializeField] private GameObject shadow2;

    private int currentScene;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        instance = this;

        if (!networkingOn && startFromScene)
        {
            SetPlayers();
            MakeShadows();
        }

        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        if (startFromScene)
        {
            LoadNextLevel();
        }
    }

    void Update()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene != scene)
        {
            currentScene = scene;
            LoadNextLevel();
        }

        CopyAndSendPlayerInfo();
    }

    public void updateFromNetworkVariables(GameObject player1, GameObject player2, GameObject shadow1, GameObject shadow2) {
        this.player1 = player1;
        this.player2 = player2;
        this.shadow1 = shadow1;
        this.shadow2 = shadow2;
    }

    private void SetPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            PlayerSettings player = p.GetComponent<PlayerSettings>();
            if (player.world1)
            {
                player1 = p;
            } else
            {
                player2 = p;
            }
        }
    }

    public void MakeShadows()
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
            MovementWASD playerMovement = player1.gameObject.GetComponent<MovementWASD>();
            playerMovement.QuantumLockAddMomentum(momentum);
        }
    }

    public void LoadNextLevel()
    {
        if (!networkingOn)
        {
            SetPlayers();
            MakeShadows();
        }

        CopyAndSendWorldInfo();
        SetCameras();
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
        if (player1 == null || player2 == null || shadow1 == null || shadow2 == null)
        {
            return;
        }

        // rn the position is done by just making the shadow under the prefab
        if (player1 != null) {
            shadow1.transform.position = player1.transform.position + new Vector3(32, 0, 0);
/*            SpriteRenderer one = shadow1.GetComponent<SpriteRenderer>();
            one.sprite = player1.GetComponent<SpriteRenderer>().sprite;*/
        }
        if (player2 != null) {
            shadow2.transform.position = player2.transform.position + new Vector3(-32, 0, 0);
/*            SpriteRenderer two = shadow2.GetComponent<SpriteRenderer>();
            two.sprite = player2.GetComponent<SpriteRenderer>().sprite;*/
        }
    }

    public GameObject GetPlayer(int num)
    {
        if (num == 1)
        {
            return player1;
        } else if (num == 2)
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

    public void SetPlayerAndShadow(GameObject player, GameObject shadow, int num)
    {
        if (num == 1)
        {
            player1 = player;
            shadow1 = shadow;
            print("set player 1");
        }
        else if (num == 2) {
            player2 = player;
            shadow2 = shadow;
        }
    }

    private void SetCameras()
    {
        // find the cameras
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");

        Camera player1camera = null;
        Camera player2camera = null;

        foreach (GameObject c in cameras)
        {
            Camera camera = c.GetComponent<Camera>();
            int world1layer = LayerMask.NameToLayer("World1");
            int world2layer = LayerMask.NameToLayer("World2");

            if (c.layer == world1layer)
            {
                player1camera = camera;
            }
            else if (c.layer == world2layer)
            {
                player2camera = camera;
            }
        }

        if (networkingOn)
        {
            //change this to actually be the person they play
            if (networkManager != null && networkManager.IsHost)
            {
                player1camera.enabled = true;
                player2camera.enabled = false;

                //edit camera locations on display
                player1camera.rect = new Rect(0, 0, 1, 1);
            }
            else
            {
                player1camera.enabled = false;
                player2camera.enabled = true;
                player2camera.rect = new Rect(0, 0, 1, 1);
            }

        } else
        {
            player1camera.enabled = true;
            player2camera.enabled = true;

            //edit camera locations on display
            player1camera.rect = new Rect(0, 0, 0.5f, 1);
            player2camera.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
    }

    public void SetNetworked(bool networked)
    {
        networkingOn = networked;

        if (networkingOn)
        {
            Screen.SetResolution(640, 480, false);

        } else
        {
            Screen.SetResolution(1280, 480, false);
        }
    }

    public bool IsNetworked() {
        return networkingOn;
    }
}
