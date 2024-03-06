using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameObject shadowPrefab;
    
    [SerializeField] private bool networkingOn = false;
    public bool startFromScene = true;

    private GameObject shadow1;
    private GameObject shadow2;

    private GameObject w1Copy;
    private GameObject w2Copy;
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
    void OnEnable()
    {
        //if (networkingOn)
        //{
        //    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetUpLevel;
        //} else
        //{
        //    SceneManager.sceneLoaded += SetUpLevel;
        //}
        SceneManager.sceneLoaded += SetUpLevel;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SetUpLevel;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            w2Copy.gameObject.SetActive(!w2Copy.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            w1Copy.gameObject.SetActive(!w1Copy.gameObject.activeSelf);
        }
    }


    public void SetUpLevel(Scene scene, LoadSceneMode mode)
    {
        if (PlayerManager.instance == null) return;
        PlayerManager.instance.SetPlayers();
        PlayerManager.instance.MakeShadows();

        CopyAndSendWorldInfo();
        SetCameras();
    }

    public void SetUpLevel(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        SetUpLevel(SceneManager.GetSceneByName(sceneName), loadSceneMode);
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

        w1Copy.SetActive(w1Open);
        w2Copy.SetActive(w2Open);
    }

    private void CopyHelper(int world)
    {
        GameObject level = GameObject.FindGameObjectWithTag("World"+world+"Level");
        GameObject transferLevel = Instantiate(level);
        int layer = LayerMask.NameToLayer("World"+world);
        transferLevel.layer = layer;
        transferLevel.transform.parent = level.transform.parent;

        int direction = world == 1 ? 1 : -1;
        transferLevel.transform.position = level.transform.position + new Vector3(32 * direction, 0, 0);

        ChangeLayerAndOpacity(transferLevel, layer);

        if (world == 1)
        {
            w1Copy = transferLevel;
        } else if (world == 2)
        {
            w2Copy = transferLevel;
        }
    }

    private void ChangeLayerAndOpacity(GameObject go, int layer)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            ChangeLayerAndOpacity(go.transform.GetChild(i).gameObject, layer);
        }

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        Tilemap tm = go.GetComponent<Tilemap>();
        if (sr)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, overlayAlpha);
            sr.sortingOrder = 1;
        } else if (tm)
        {
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, overlayAlpha);
            tm.GetComponent<TilemapRenderer>().sortingOrder = 1;
        }
        go.layer = layer;
        return;
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

    //public void SetPlayerAndShadow(GameObject player, GameObject shadow, int num) { PlayerManager.instance.SetPlayerAndShadow(player, shadow, num); }
}
