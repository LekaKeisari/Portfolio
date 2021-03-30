using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
using System.Threading.Tasks;
using UnityEngine.UI;
using Valve.VR;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private FireVFX fireVFX;
    private SteamVR_LaserPointer leftHandLaserPointer;
    private SteamVR_LaserPointer rightHandLaserPointer;
    [SerializeField] private ScenarioManagerSO scenarioManager = null;
    [SerializeField] private GameObject extinguisherSpawnPoint = null;    
    [SerializeField] private GameObject extinguisher2SpawnPoint = null;
    [SerializeField] private Canvas instructionCanvas;
    [SerializeField] private Canvas extinguisherCanvas;
    private AudioSource audioSrc;

    private void Awake()
    {
        leftHandLaserPointer = Player.instance.GetComponentsInChildren<SteamVR_LaserPointer>()[0];
        rightHandLaserPointer = Player.instance.GetComponentsInChildren<SteamVR_LaserPointer>()[1];        
        scenarioManager.FeedbackCanvas = this.GetComponent<Canvas>();
        scenarioManager.ExtinguisherSpawnPoint = extinguisherSpawnPoint;
        scenarioManager.Extinguisher2SpawnPoint = extinguisher2SpawnPoint;
    }
    private void Start()
    {
        scenarioManager.SetupScenarioManager();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        audioSrc = GetComponent<AudioSource>();
        audioSrc.Play();
        if (scenarioManager.CurrentTask == ScenarioManagerSO.task.Task3)
        {
            ActivateLaserPointers();
        }
    }

    public void ActivateLaserPointers()
    {
        leftHandLaserPointer.SetActive(true);
        rightHandLaserPointer.SetActive(true);
    }

    public void DeactivateLaserPointers()
    {
        leftHandLaserPointer.SetActive(false);
        rightHandLaserPointer.SetActive(false);
    }



    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        leftHandLaserPointer.SetActive(true);
        rightHandLaserPointer.SetActive(true);        
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public IEnumerator StartNextTask()
    {
        yield return new WaitForSeconds(5f);
        SteamVR_Fade.View(Color.black, fadeTime);
        yield return new WaitForSeconds(fadeTime);
        instructionCanvas.gameObject.SetActive(true);
        extinguisherCanvas.gameObject.SetActive(true);
        scenarioManager.PickNextTask();
        scenarioManager.SetupTask();
        scenarioManager.PlayerPosResetter.ResetPosition();
        SteamVR_Fade.View(Color.clear, fadeTime);
        fireVFX.ResetTimer();
        ActivateLaserPointers();
        gameObject.SetActive(false);
    }
}
