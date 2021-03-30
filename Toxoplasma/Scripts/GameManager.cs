using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{    
    
    public bool randomBattles = true;

    public bool playerMoveAllowed = true;
    public bool dangerZone = true;
    private bool gamePaused = false;

    public Interactable activeIntreractable;

    public static GameManager instance = null;

    //[HideInInspector]
    public CameraMovement mainCamera;

    
    public GameObject menuCanvas;
    public GameObject fadeCanvas;

    //[HideInInspector]
    public GameObject player;
    public GameObject camTarget;
    [HideInInspector]
    public Vector3 playerPosition = Vector3.zero;
    [HideInInspector]
    public Quaternion playerRotation;
    [HideInInspector]
    public Vector3 cameraTargetPosition;
    [HideInInspector]
    public Quaternion cameraRotation;
    [HideInInspector]
    public Quaternion cameraTargetRotation;
    public bool ableToClimb = false;

    [HideInInspector]
    public bool startEventDone = false;
    [HideInInspector]
    public bool returnCameraMovement = false;

    [HideInInspector]
    public bool moveEventInProgress = false;

    public int battleZoneNumber = 1;

    private Coroutine randomBattleCoroutine;

    [HideInInspector]
    public bool cameraFollowDisabled = true;

    public Dictionary<string, bool> oneTimeEventTriggered = new Dictionary<string, bool>();

    public List<ScriptedEvent> scriptedEvents = new List<ScriptedEvent>();

    [HideInInspector]
    public DialogueManager dialogueManager;

    [HideInInspector]
    public Vector3 mainCameraOffset = Vector3.zero;

    [HideInInspector]
    public GameObject nPC;

    public Texture2D cursorImage;

    private AudioManager audioManager;

    private Vector3 playerDefaultPosition = Vector3.zero;
    private Vector3 cameraDefaultPosition = Vector3.zero;

    [HideInInspector]
    public bool restarted = false;

    void Awake()
    {
        //If this script does not exit already, use this current instance
        if (instance == null)
            instance = this;

        //If this script already exit, DESTROY this current instance
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        SavePositions();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (randomBattles && SceneManager.GetActiveScene().buildIndex == 0)
        {
            randomBattleCoroutine = StartCoroutine(RandomBattle());
        }

        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.Auto);
        audioManager = AudioManager.instance;

        playerDefaultPosition = player.transform.position;
        cameraDefaultPosition = mainCamera.transform.position;

        StartCoroutine(SetupOneTimeEvents());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActionButtonHandler();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        restarted = true;
        //mainCamera.transform.position = cameraDefaultPosition;
        playerPosition = playerDefaultPosition;
        playerRotation = Quaternion.identity;
        startEventDone = false;
        StopAllCoroutines();
        //foreach (ScriptedEvent scriptedEvent in scriptedEvents)
        //{
        //    scriptedEvent.eventTriggered = false;
        //}
        List<string> keys = new List<string>();
        //scriptedEvents = null;
        //oneTimeEventTriggered = null;
        foreach (KeyValuePair<string, bool> keyValuePair in oneTimeEventTriggered)
        {
            keys.Add(keyValuePair.Key);
        }

        foreach (string key in keys)
        {
            oneTimeEventTriggered[key] = false;
        }
        ToggleRandomBattle(false);

        //PlayerStats playerStats = player.GetComponent<PlayerStats>();
        //playerStats.PlayerCurrentMaxHealth = 50;
        //playerStats.PlayerCurrentHealth = 50;
        //playerStats.CurrentXP = 0;
        //playerStats.NextLevelXP = 10;
        //playerStats.PlayerLevel = 1;
    

        audioManager.ResetAudioManager();
        SceneManager.LoadScene(0);

        //StartCoroutine(SetupOneTimeEvents());

    }

    private IEnumerator RandomBattle()
    {
        yield return new WaitForSeconds(Random.Range(10, 15));
        if (player)
        {
            SavePositions();
        }
        if (!cameraFollowDisabled)
        {
            returnCameraMovement = true;
        }
        else
        {
            returnCameraMovement = false;
        }
        //dangerZone = false;
        SceneManager.LoadScene(1);
    }

    public void ToggleRandomBattle(bool on)
    {
        if (!on)
        {
            randomBattles = false;
            if (randomBattleCoroutine != null)
            {
                StopCoroutine(randomBattleCoroutine);
            }
            if (SceneManager.GetActiveScene().buildIndex == 1 && !restarted)
            {
                StartCoroutine(BattleManager.instance.BackToWorld());
            }
        }
        if (on)
        {
            randomBattles = true;
            randomBattleCoroutine = StartCoroutine(RandomBattle());
        }        
    }

    private void HandleEscape()
    {
        PauseMenuHandler();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseMenuHandler()
    {
        if (gamePaused)
        {
            menuCanvas.SetActive(false);
            Time.timeScale = 1;
            gamePaused = false;
            Cursor.visible = false;
        }
        else
        {
            menuCanvas.SetActive(true);
            Time.timeScale = 0;
            gamePaused = true;
            Cursor.visible = true;
        }
    }

    private void SavePositions()
    {
        playerPosition = player.transform.position;
        playerRotation = player.transform.rotation;
        if (cameraFollowDisabled)
        {
            cameraTargetPosition = mainCamera.transform.position;
            cameraRotation = mainCamera.transform.localRotation;
            cameraTargetRotation = mainCamera.transform.parent.rotation;
        }
        else
        {
            cameraTargetPosition = Vector3.zero;
            mainCameraOffset = mainCamera.transform.localPosition;
            cameraTargetRotation = mainCamera.transform.parent.rotation;
        }
    }

    private void ActionButtonHandler()
    {
        if (ableToClimb && activeIntreractable && playerMoveAllowed)
        {
            activeIntreractable.Climb();
        }
        
    }

    private IEnumerator SetupOneTimeEvents()
    {
        yield return new WaitForSeconds(0.1f);

        if (oneTimeEventTriggered != null && scriptedEvents != null)
        {
            foreach (ScriptedEvent scriptedEvent in scriptedEvents)
            {
                foreach (KeyValuePair<string, bool> keyValuePair in oneTimeEventTriggered)
                {
                    if (keyValuePair.Value == true && scriptedEvent.gameObject.name == keyValuePair.Key)
                    {
                        //Debug.Log("value check");
                        scriptedEvent.eventTriggered = true;
                    }
                }
            }
        }
    }

    public void CallSetupEventsCoroutine()
    {
        StartCoroutine(SetupOneTimeEvents());
    }
}
