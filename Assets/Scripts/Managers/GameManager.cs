using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public NetworkManager networkManager;
    public GameObject shadowPrefab;
    
    [SerializeField] private bool networkingOn = false;
    public bool startFromScene = true;

    private int currentScene;
    public GameObject shadowPrefab;

    [SerializeField] private GameObject shadow1;
    [SerializeField] private GameObject shadow2;

    private List<SpriteRenderer> w1SpritesCopy = new List<SpriteRenderer>();
    private TilemapRenderer w1TilemapCopy;
    private List<SpriteRenderer> w2SpritesCopy = new List<SpriteRenderer>();
    private TilemapRenderer w2TilemapCopy;
    private float overlayAlpha = 0.3f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        instance = this;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += SetUpLevel;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SetUpLevel;
    }

    void Update()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            w2TilemapCopy.enabled = !w2TilemapCopy.enabled;
            foreach (SpriteRenderer sr in w2SpritesCopy) {
                sr.enabled = !sr.enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            w1TilemapCopy.enabled = !w1TilemapCopy.enabled;
            foreach (SpriteRenderer sr in w1SpritesCopy) {
                sr.enabled = !sr.enabled;
            }
        }
        //CopyAndSendPlayerInfo();
    }


    public void SetUpLevel(Scene scene, LoadSceneMode mode)
    {
        PlayerManager.instance.SetPlayers();
        PlayerManager.instance.MakeShadows();

        CopyAndSendWorldInfo();
        SetCameras();
    }

    public void CopyAndSendWorldInfo()
    {
        GameObject level = GameObject.FindGameObjectWithTag("World1Level");
        GameObject transferLevel = Instantiate(level);
        transferLevel.layer = LayerMask.NameToLayer("World1");
        transferLevel.transform.parent = level.transform.parent;
        transferLevel.transform.position = level.transform.position + new Vector3(32, 0, 0);

        Transform tilemapLevel = transferLevel.transform.Find("Tilemap_level");
        Tilemap tm = tilemapLevel.GetComponent<Tilemap>();
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, overlayAlpha);
        w1TilemapCopy = tilemapLevel.GetComponent<TilemapRenderer>();
        for (int i = 0; i < transferLevel.transform.childCount; i++)
        {
            GameObject child = transferLevel.transform.GetChild(i).gameObject;
            child.layer = LayerMask.NameToLayer("World1");

            Collider2D collider = child.GetComponent<Collider2D>();
            if (collider)
            {
                collider.enabled = false;
            }

            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, overlayAlpha);
                w1SpritesCopy.Add(sr);
            }
        }

        level = GameObject.FindGameObjectWithTag("World2Level");
        transferLevel = Instantiate(level);
        transferLevel.layer = LayerMask.NameToLayer("World2");
        transferLevel.transform.parent = level.transform.parent;
        transferLevel.transform.position = level.transform.position + new Vector3(-32, 0, 0);

        tilemapLevel = transferLevel.transform.Find("Tilemap_level");
        tm = tilemapLevel.GetComponent<Tilemap>();
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, overlayAlpha);
        w2TilemapCopy = tilemapLevel.GetComponent<TilemapRenderer>();
        for (int i = 0; i < transferLevel.transform.childCount; i++)
        {
            GameObject child = transferLevel.transform.GetChild(i).gameObject;
            child.layer = LayerMask.NameToLayer("World1");

            Collider2D collider = child.GetComponent<Collider2D>();
            if (collider)
            {
                collider.enabled = false;
            }

            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, overlayAlpha);
                w2SpritesCopy.Add(sr);
            }
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

    // To deal with external functions that still call Game Manager. Should be refactored to be removed but still may be necessary for older scenes. 
    public GameObject GetPlayer(int num) { return PlayerManager.instance.GetPlayer(num); }
    public GameObject GetShadow(int num) { return PlayerManager.instance.GetShadow(num); }
    public void MakeShadows() { PlayerManager.instance.MakeShadows(); }
    public void SendMomentum(Vector2 momentum, GameObject sender) { PlayerManager.instance.SendMomentum(momentum, sender); }

    public void SetPlayerAndShadow(GameObject player, GameObject shadow, int num) { PlayerManager.instance.SetPlayerAndShadow(player, shadow, num); }
}
