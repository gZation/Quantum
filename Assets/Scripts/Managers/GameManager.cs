using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using Unity.Collections.LowLevel.Unsafe;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    public GameObject shadowPrefab;
    public GameObject copyGrid;

    // Should only be set to true when the main gameplay loop is running
    [SerializeField] public bool isGameEnabled = true;
    [SerializeField] public bool networkingOn = false;
    public bool startFromScene = true;
    public bool cutscene;

    private GameObject w1Copy;
    private GameObject w2Copy;
    private bool leftToggle;
    private bool rightToggle;

    private float overlayAlpha = 0.3f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // If GameManager is manually set to gameEnabled, we need to initialize the Game Manager and Player Manager. 
        if (startFromScene)
        {
            GameEnable();
            SetUpLevel();
        }

        //if (isGameEnabled) GameEnable();
    }
    public void GameEnable()
    {
        //print("GameEnable");
        leftToggle = true;
        rightToggle = true;
        isGameEnabled = true;
        //print($"Networking on? {networkingOn}");
        if (!networkingOn) SceneManager.sceneLoaded += SetUpLevel;
        else NetworkManager.Singleton.SceneManager.OnLoadComplete += SetUpLevel;
    }


    public void SetNetworkedScreen(bool networked)
    {
        networkingOn = networked;
        
        if (networkingOn)
        {
            Screen.SetResolution(640, 480, false);
        }
        else
        {
            Screen.SetResolution(1280, 480, false);
        }
    }

    public void SetSceneLoad()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SetUpLevel;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SetUpLevel;
    }

    void Update()
    {
        if (IsNetworked())
        {
            if (Input.GetButtonDown("Toggle"))
            {
                int currPlayer = PlayerManager.instance.currPlayer;

                if (currPlayer == 1)
                {
                    leftToggle = !w2Copy.gameObject.activeSelf;
                    w2Copy.gameObject.SetActive(leftToggle);
                } else if (currPlayer == 2)
                {
                    rightToggle = !w1Copy.gameObject.activeSelf;
                    w1Copy.gameObject.SetActive(rightToggle);
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("ToggleLeft"))
            {
                ToggleOverlay(true);
            }
            if (Input.GetButtonDown("ToggleRight"))
            {
                ToggleOverlay(false);
            }
        }
        if (Input.GetButton("Restart"))
        {
            LevelLoader.instance.ReloadLevel();
        }
    }

    public void ToggleOverlay(bool world1)
    {
        // For Future Programmer reference: w#copy is the copy of the opposing world,
        // not the current world doing the toggling
        if (world1)
        {
            leftToggle = !w2Copy.gameObject.activeSelf;
            w2Copy.gameObject.SetActive(leftToggle);
        }
        else
        {
            rightToggle = !w1Copy.gameObject.activeSelf;
            w1Copy.gameObject.SetActive(rightToggle);
        }
    }

    public void SetUpLevel()
    {
        // Don't set up the level if PlayerManager doesn't exist
        //print($"PlayerManager: ${PlayerManager.instance.name}");
        if (PlayerManager.instance == null) return;

        //Don't set up the level if the players don't exist
        if (!cutscene && !PlayerManager.instance.SetPlayersAndShadows())
        {
            //print($"SetUpLevel Failed. Cutscene ${cutscene}");
            return;
        };

        if (networkingOn)
        {
            Screen.SetResolution(640, 480, false);
        }
        else
        {
            Screen.SetResolution(1280, 480, false);
        }

        if (!cutscene)
        {
            CopyAndSendWorldInfo();

            // reset world toggle as needed
            w2Copy.gameObject.SetActive(leftToggle);
            w1Copy.gameObject.SetActive(rightToggle);
        }

        SetCameras();
    }

    public void SetUpLevel(Scene scene, LoadSceneMode mode) {
        SetUpLevel(); 
    }

    public void SetUpLevel(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SetUpLevel();
        }
    }


    public void CopyAndSendWorldInfo()
    {
        bool w1Open = w1Copy == null ? true : w1Copy.activeSelf;
        bool w2Open = w2Copy == null ? true : w2Copy.activeSelf;

        if (w1Copy != null)
        {
            Destroy(w1Copy.gameObject);
        }
        
        if (w2Copy != null)
        {
            Destroy(w2Copy.gameObject);
        }

        CopyHelper(1);
        CopyHelper(2);

        if (w1Copy != null && w2Copy != null)
        {
            w1Copy.SetActive(w1Open);
            w2Copy.SetActive(w2Open);
        }
    }

    private void CopyHelper(int world)
    {
        GameObject level = GameObject.FindGameObjectWithTag("World"+world+"Level");

        if (level == null)
        {
            return;
        }

        GameObject transferLevel = Instantiate(copyGrid);
        int layer = LayerMask.NameToLayer("World"+world);
        transferLevel.layer = layer;

        int direction = world == 1 ? 1 : -1;
        transferLevel.transform.position = level.transform.position + new Vector3(32 * direction, 0, 0);
        transferLevel.transform.parent = level.transform.parent;

        GameObject shadow = ChangeLayerAndOpacity(transferLevel, level, layer, world);

        if (world == 1)
        {
            w1Copy = shadow;
        } else if (world == 2)
        {
            w2Copy = shadow;
        }
    }

    private GameObject ChangeLayerAndOpacity(GameObject transferLevelGo, GameObject levelGo, int layer, int world)
    {
        if (!levelGo.activeSelf)
        {
            return null;
        }

        SpriteRenderer levelSR = levelGo.GetComponent<SpriteRenderer>();
        Tilemap levelTM = levelGo.GetComponent<Tilemap>();
        Grid grid = levelGo.GetComponent<Grid>();
        GameObject levelshadow = transferLevelGo;
        int direction = world == 1 ? 1 : -1;


        if (levelTM)
        {
            levelshadow = Instantiate(levelGo);
            Tilemap transferTM = levelshadow.GetComponent<Tilemap>();
            transferTM.color = new Color(levelTM.color.r, levelTM.color.g, levelTM.color.b, overlayAlpha);
            transferTM.GetComponent<TilemapRenderer>().sortingOrder = 1;
            levelshadow.transform.parent = transferLevelGo.transform;
        }
        else if (!grid)
        {
            levelshadow = new GameObject("LevelAssetShadow");
            levelshadow.transform.parent = transferLevelGo.transform;
            levelshadow.transform.localScale = levelGo.transform.localScale;
            levelshadow.transform.position = levelGo.transform.position + new Vector3(32 * direction, 0, 0);
            levelshadow.transform.rotation = levelGo.transform.rotation;

            if (levelSR)
            {
                SpriteRenderer transferSR = levelshadow.AddComponent<SpriteRenderer>();
                transferSR.sprite = levelSR.sprite;
                transferSR.color = new Color(transferSR.color.r, transferSR.color.g, transferSR.color.b, overlayAlpha);
                transferSR.adaptiveModeThreshold = levelSR.adaptiveModeThreshold;
                transferSR.drawMode = levelSR.drawMode;
                transferSR.size = levelSR.size;
                transferSR.tileMode = levelSR.tileMode;
                transferSR.sortingLayerName = "entities";
                transferSR.sortingOrder = 1;
            }
        }

        LevelAssetShadow shadow = levelshadow.AddComponent<LevelAssetShadow>();
        shadow.parent = levelGo;
        shadow.offset = world == 1 ? new Vector3(32, 0, 0) : new Vector3(-32, 0, 0); 

        levelshadow.layer = layer;

        for (int i = 0; i < levelGo.transform.childCount; i++)
        {
            ChangeLayerAndOpacity(levelshadow, levelGo.transform.GetChild(i).gameObject, layer, world);
        }

        return levelshadow;
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

        if (player1camera == null)
        {
            return;
        }

        if (networkingOn)
        {
            if (PlayerManager.instance.currPlayer == 1)
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

            if (PlayerManager.instance.playerOnLeft == 2)
            {
                player1camera.rect = new Rect(0.5f, 0, 0.5f, 1);
                player2camera.rect = new Rect(0, 0, 0.5f, 1);
            }
            else
            {
                player1camera.rect = new Rect(0, 0, 0.5f, 1);
                player2camera.rect = new Rect(0.5f, 0, 0.5f, 1);
            }
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

    //public void SetPlayerAndShadow(GameObject player, GameObject shadow, int num) { PlayerManager.instance.SetPlayerAndShadow(player, shadow, num); }
}
