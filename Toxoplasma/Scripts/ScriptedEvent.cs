using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScriptedEvent : MonoBehaviour
{    
    [SerializeField]
    private CameraMovement mainCamera;

    private GameObject camTarget;

    private GameManager gameManager;
    private BattleManager battleManager;

    [SerializeField]
    private float cameraMoveSpeed = 0.01f;
    [SerializeField]
    private float cameraRotationSpeed = 0.01f;

    [SerializeField]
    private float cameraDistanceToWaypoint = 0.1f;

    [SerializeField]
    private bool stopPlayerMovement = false;

    [SerializeField]
    private bool npcMovementAllowed = false;

    [SerializeField]
    private bool triggerOnlyOnce = false;

    [SerializeField]
    private bool dontMoveCamera = false;
    [SerializeField]
    private bool saveLastCameraOffset = false;
    [SerializeField]
    private bool saveFirstCameraOffset = false;
    [SerializeField]
    private bool useCustomOffsets = false;

    [SerializeField]
    private Vector3 customCameraOffset;
    [SerializeField]
    private float customCameraRotation;

    [SerializeField]
    private bool triggerNextBattleGround = false;
      
    public bool eventTriggered = false;

    public bool eventLocked = false;
    [SerializeField]
    private ScriptedEvent scriptedEventToUnlock;

    private Animator playerAnimator;
    private Animator npcAnimator;

    [SerializeField]
    private List<GameObject> cameraWaypoints = new List<GameObject>();

    [SerializeField]
    private List<GameObject> npcWaypoints = new List<GameObject>();
    
    [SerializeField]
    private List<GameObject> playerWaypoints = new List<GameObject>();

    private GameObject player;
    private GameObject nPC;

    [SerializeField]
    private float playerDialogueOffsetX = -200f;
    [SerializeField]
    private float playerDialogueOffsetY = 100f;
    [SerializeField]
    private float npcDialogueOffsetX = 200f;
    [SerializeField]
    private float npcDialogueOffsetY = 100f;

    private GameObject fadeCanvas;
    private Image fadeImage;

    private bool fadeInProgress = false;

    [SerializeField]
    private ScriptedEvent nextStoryEvent;

    [SerializeField]
    private GameObject teleportPosition;

    [SerializeField]
    private bool needsInput = false;

    private AudioSource audioSource;
    
    //public enum EventCameraState
    //{
    //    NotStarted,
    //    EventStarted,
    //    FollowWaypoints,
    //    StayAtLastWaypoint,
    //    Teleport,
    //    FollowPlayer,
    //    EventOver
    //}

    //private EventCameraState currentEventCameraState = EventCameraState.NotStarted;

    //[SerializeField]
    //private EventCameraState finalCameraState = EventCameraState.FollowPlayer;

    void Start()
    {
        gameManager = GameManager.instance;
        battleManager = BattleManager.instance;

        player = gameManager.player;

        if (battleManager)
        {
            playerAnimator = battleManager.player.GetComponentInChildren<Animator>();

        }
       

        if (triggerOnlyOnce && !gameManager.oneTimeEventTriggered.ContainsKey(name))
        {
            gameManager.oneTimeEventTriggered.Add(name, false);
            gameManager.scriptedEvents.Add(this);
        }

        if (gameManager.oneTimeEventTriggered.ContainsKey(name) && gameManager.oneTimeEventTriggered[name])
        {
            eventTriggered = true;
        }



        //fadeCanvas = GameManager.instance.fadeCanvas;
        //fadeImage = fadeCanvas.GetComponentInChildren<Image>();
        //fadeImage.canvasRenderer.SetAlpha(0.01f);

        audioSource = AudioManager.instance.GetComponent<AudioSource>();

        StartCoroutine(EventStarter());
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player" && !eventLocked/* && currentEventCameraState == EventCameraState.NotStarted*/)
        {
            StartCoroutine(TriggerEnterEventStarter());
        }
    }

    //private void SetCameraState(EventCameraState currentState)
    //{
    //    switch (currentState)
    //    {
    //        case EventCameraState.NotStarted:
    //            break;
    //        case EventCameraState.EventStarted:
    //            break;
    //        case EventCameraState.FollowWaypoints:
    //            break;
    //        case EventCameraState.StayAtLastWaypoint:
    //            break;
    //        case EventCameraState.Teleport:
    //            break;
    //        case EventCameraState.FollowPlayer:
    //            break;
    //        case EventCameraState.EventOver:
    //            break;
    //        default:
    //            break;
    //    }
    //    currentEventCameraState = currentState;
    //}

    private IEnumerator BeginEvent()
    {
        //SetCameraState(EventCameraState.EventStarted);
        yield return new WaitForSeconds(0.01f);
        mainCamera = gameManager.mainCamera;
        camTarget = mainCamera.cameraTarget;
        if (triggerOnlyOnce && eventTriggered)
        {
            //Debug.Log("toimii");
            gameObject.SetActive(false);
            yield return null;
        }
        if (triggerOnlyOnce && !eventTriggered)
        {
            eventTriggered = true;
            gameManager.oneTimeEventTriggered[name] = true;
        }
        gameManager.cameraFollowDisabled = true;


        if (stopPlayerMovement)
        {
            gameManager.playerMoveAllowed = false;
        }

        if (gameManager.dangerZone && SceneManager.GetActiveScene().buildIndex != 1)
        {
            gameManager.ToggleRandomBattle(false);
        }
            Debug.Log(name);

        foreach (GameObject waypoint in cameraWaypoints)
        {
            if (!dontMoveCamera)
            {
                while (Vector3.Distance(mainCamera.transform.position, waypoint.transform.position) > cameraDistanceToWaypoint && mainCamera.transform.rotation != waypoint.transform.rotation)
                {
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, waypoint.transform.position, cameraMoveSpeed);
                    mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, waypoint.transform.rotation, cameraRotationSpeed);
                    yield return new WaitForSeconds(0.01f);
                }

            }
            if (waypoint.GetComponent<DialogueTrigger>())
            {
                AssignDialogueManager(waypoint.GetComponent<DialogueTrigger>());
                DialogueTrigger waypointDialogue = waypoint.GetComponent<DialogueTrigger>();
                waypointDialogue.dialogueCounter = 0;
                waypointDialogue.dialoguePossible = true;
                waypointDialogue.callFromEvent = true;
                Coroutine printDialogue = StartCoroutine(waypointDialogue.PrintDialogue());
                yield return new WaitUntil(() => waypointDialogue.dialogueManager.dialogueDone);
                waypointDialogue.dialoguePossible = false;
                waypointDialogue.callFromEvent = false;                
            }
        }
        if (saveLastCameraOffset)
        {
            //camTarget.transform.rotation = waypoint.transform.rotation;
            camTarget.transform.rotation = Quaternion.Euler(0, cameraWaypoints[cameraWaypoints.Count-1].transform.rotation.eulerAngles.y, 0);
            mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            mainCamera.transform.position = cameraWaypoints[cameraWaypoints.Count - 1].transform.position;
        }
        if (saveFirstCameraOffset)
        {
            //camTarget.transform.rotation = cameraWaypoints[0].transform.rotation;
            camTarget.transform.rotation = Quaternion.Euler(0, cameraWaypoints[0].transform.rotation.eulerAngles.y, 0);
            mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            mainCamera.transform.position = cameraWaypoints[0].transform.position;
        }
        if (!saveLastCameraOffset && !saveFirstCameraOffset && !dontMoveCamera)
        {
            gameManager.cameraFollowDisabled = false;
        }        

        if (stopPlayerMovement)
        {
            gameManager.playerMoveAllowed = true;
        }

        if (gameManager.dangerZone && SceneManager.GetActiveScene().buildIndex != 1)
        {
            //Debug.Log("random");
            gameManager.ToggleRandomBattle(true);
        }
        if (!gameManager.startEventDone)
        {
            gameManager.startEventDone = true;
        }
        if (tag == "BattleStartEvent")
        {
            if (saveLastCameraOffset)
            {
                mainCamera.transform.localRotation = Quaternion.Euler(cameraWaypoints[cameraWaypoints.Count - 1].transform.localRotation.eulerAngles.x, 0, 0);
            }
            battleManager.battleStartEventDone = true;
            playerAnimator.SetBool("isBattling", true);
        }
        if (useCustomOffsets)
        {
            //StartCoroutine(Fade(10f, false));
            camTarget.transform.position = GameManager.instance.player.transform.position;
            mainCamera.transform.localPosition = customCameraOffset;
            camTarget.GetComponent<FollowPlayer>().cameraRotation = customCameraRotation;
            //StartCoroutine(Fade(5f, true));
        }

        if (nextStoryEvent)
        {
            nextStoryEvent.StartNextStoryEvent();
        }

            if (scriptedEventToUnlock)
        {
            UnlockEvent(scriptedEventToUnlock);
        }
    }

    private IEnumerator EventStarter()
    {
        yield return new WaitForSeconds(0.3f);

        if (tag == "SceneStartEvent" && !gameManager.startEventDone)
        {
            StartCoroutine(BeginEvent());
            //gameManager.startEventDone = true;
            //mainCamera.cameraFollowDisabled = false;
        }
        if (tag == "BattleStartEvent")
        {
            battleManager.battleStartEventDone = false;
            StartCoroutine(BeginEvent());
        }
    }

    private IEnumerator TriggerEnterEventStarter()
    {
        yield return new WaitForSeconds(0.1f);

        if (triggerOnlyOnce && eventTriggered)
        {
            Debug.Log("toimii");
            yield break;
        }

        if (triggerNextBattleGround)
        {
            gameManager.battleZoneNumber++;
        }
        switch (tag)
        {
            case "TestEvent":
                StartCoroutine(BeginEvent());
                break;
            case "DangerZoneEvent":
                gameManager.dangerZone = true;
                //gameManager.randomBattles = false;
                StartCoroutine(BeginEvent());
                break;
            case "DangerZoneEndsEvent":
                gameManager.dangerZone = false;
                gameManager.ToggleRandomBattle(false);
                StartCoroutine(BeginEvent());
                break;
            case "InteractableEvent":
                if (needsInput)
                {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

                }
                StartCoroutine(BeginStoryEvent());
                break;
            default:
                break;
        }
    }
       
    private void UnlockEvent(ScriptedEvent scriptedEvent)
    {
        scriptedEvent.eventLocked = false;
    }

    private IEnumerator Fade(float duration, bool fadeOut)
    {
        yield return new WaitUntil(() => !fadeInProgress);

        if (fadeOut)
        {
            fadeInProgress = true;
            fadeCanvas.SetActive(true);
            fadeImage.CrossFadeAlpha(0f, duration, false);

            yield return new WaitForSeconds(duration);

            fadeCanvas.SetActive(false);
            fadeInProgress = false;
        }
        else
        {
            fadeInProgress = true;
            fadeCanvas.SetActive(true);
            fadeImage.CrossFadeAlpha(1f, duration, false);

            yield return new WaitForSeconds(duration);

            fadeCanvas.SetActive(false);
            fadeInProgress = false;
        }

    }

    private IEnumerator BeginStoryEvent()
    {
        if (triggerOnlyOnce && eventTriggered)
        {
            //Debug.Log("toimii");
            gameObject.SetActive(false);
            yield return null;
        }

        if (triggerOnlyOnce && !eventTriggered)
        {
            eventTriggered = true;
            gameManager.oneTimeEventTriggered[name] = true;
        }
        player = gameManager.player;
        nPC = gameManager.nPC;
        playerAnimator = player.GetComponent<PlayerMovement>().animator;
        npcAnimator = nPC.GetComponentInChildren<Animator>();

        gameManager.moveEventInProgress = true;
        gameManager.playerMoveAllowed = false;
        playerAnimator.SetBool("isRunning", true);
        foreach (GameObject playerWaypoint in playerWaypoints)
        {

            while (Vector3.Distance(player.transform.position, playerWaypoint.transform.position) > cameraDistanceToWaypoint /*&& player.transform.rotation != playerWaypoint.transform.rotation*/)
            {
                player.transform.position = Vector3.Lerp(player.transform.position, playerWaypoint.transform.position, cameraMoveSpeed);
                player.transform.rotation = playerWaypoint.transform.rotation;
                yield return new WaitForSeconds(0.01f);
            }
            if (playerWaypoint.GetComponent<DialogueTrigger>())
            {
                AssignDialogueManager(playerWaypoint.GetComponent<DialogueTrigger>());
                DialogueTrigger waypointDialogue = playerWaypoint.GetComponent<DialogueTrigger>();
                waypointDialogue.dialogueCounter = 0;
                waypointDialogue.dialoguePossible = true;
                waypointDialogue.callFromEvent = true;
                playerAnimator.SetBool("isRunning", false);
                SetDialogueBoxPosition(waypointDialogue.dialogueManager.dialogueImage, player, playerDialogueOffsetX, playerDialogueOffsetY);
                Coroutine printDialogue = StartCoroutine(waypointDialogue.PrintDialogue());
                yield return new WaitUntil(() => waypointDialogue.dialogueManager.dialogueDone);
                waypointDialogue.dialoguePossible = false;
                waypointDialogue.callFromEvent = false;
            }
        }
        gameManager.moveEventInProgress = false;                

        yield return new WaitForSeconds(0.1f);

        npcAnimator.SetBool("isRunning", true);

        if (teleportPosition)
        {
            TeleportCharacterTo(nPC, teleportPosition.transform);
        }

        foreach (GameObject npcWaypoint in npcWaypoints)
        {
            while (Vector3.Distance(nPC.transform.position, npcWaypoint.transform.position) > cameraDistanceToWaypoint && nPC.transform.rotation != npcWaypoint.transform.rotation)
            {
                nPC.transform.position = Vector3.Lerp(nPC.transform.position, npcWaypoint.transform.position, cameraMoveSpeed);
                nPC.transform.rotation = Quaternion.Lerp(nPC.transform.rotation, npcWaypoint.transform.rotation, cameraRotationSpeed);
                yield return new WaitForSeconds(0.01f);
            }
            if (npcWaypoint.GetComponent<DialogueTrigger>())
            {
                AssignDialogueManager(npcWaypoint.GetComponent<DialogueTrigger>());
                DialogueTrigger waypointDialogue = npcWaypoint.GetComponent<DialogueTrigger>();
                waypointDialogue.dialogueCounter = 0;
                waypointDialogue.dialoguePossible = true;
                waypointDialogue.callFromEvent = true;
                npcAnimator.SetBool("isRunning", false);
                SetDialogueBoxPosition(waypointDialogue.dialogueManager.dialogueImage, nPC, npcDialogueOffsetX, npcDialogueOffsetY);
                Coroutine printDialogue = StartCoroutine(waypointDialogue.PrintDialogue());
                yield return new WaitUntil(() => waypointDialogue.dialogueManager.dialogueDone);
                waypointDialogue.dialoguePossible = false;
                waypointDialogue.callFromEvent = false;
            }
        }

        yield return new WaitForSeconds(0.1f);

        if (GetComponent<VideoPlayer>())
        {
            //Debug.Log("toimii");

            VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
            audioSource.Pause();
            videoPlayer.Play();

            float startTime = Time.time;

            yield return new WaitUntil(() => Time.time > startTime + videoPlayer.clip.length);

            videoPlayer.Stop();
            audioSource.UnPause();

        }

        

        if (nextStoryEvent)
        {
            nextStoryEvent.StartNextStoryEvent();
        }
        if (scriptedEventToUnlock)
        {
            UnlockEvent(scriptedEventToUnlock);
        }

        if (npcMovementAllowed)
        {
            nPC.GetComponent<NPCMovement>().npcMovementEnabled = true;
        }
        if (!npcMovementAllowed)
        {
            nPC.GetComponent<NPCMovement>().npcMovementEnabled = false;
        }
        gameManager.playerMoveAllowed = true;

    }

    public void StartNextStoryEvent()
    {
        StartCoroutine(BeginStoryEvent());
    }

    private void AssignDialogueManager(DialogueTrigger dialogueTrigger)
    {
        if (dialogueTrigger.dialogueManager == null)
        {
            dialogueTrigger.dialogueManager = gameManager.dialogueManager;
        }
    }

    private void SetDialogueBoxPosition(RectTransform rectTransform, GameObject talker, float offsetX, float offsetY)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(talker.transform.position);
        rectTransform.anchoredPosition = new Vector2(screenPoint.x + offsetX, screenPoint.y + offsetY/*, rectTransform.rect.width, rectTransform.rect.height*/);
    }

    private void TeleportCharacterTo(GameObject gameObject, Transform teleportPosition)
    {
        gameObject.transform.position = teleportPosition.position;
    }

}
