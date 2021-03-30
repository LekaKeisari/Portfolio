using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

[CreateAssetMenu(fileName = "ScenarioManager", menuName = "ScriptableObject/ScenarioManager", order = 1)]
public class ScenarioManagerSO : ScriptableObject
{
    [SerializeField] private Canvas feedbackCanvas = null;
    public Canvas FeedbackCanvas { get => feedbackCanvas; set => feedbackCanvas = value; }
    [SerializeField] private Text feedbackText = null;
    public Text FeedbackText { get => feedbackText; set => feedbackText = value; }
    [SerializeField] private Button[] buttons = null;
    [SerializeField] private Interactable extinguisherPrefab = null;
    [SerializeField] private Interactable extinguisher2Prefab = null;
    [SerializeField] private Interactable waterHosePrefab = null;
    [SerializeField] private Interactable foamPistolPrefab = null;
    private GameObject extinguisherSpawnPoint = null;
    private GameObject extinguisher2SpawnPoint = null;

    private Interactable extinguisher = null;
    private Interactable extinguisher2 = null;
    private Interactable waterHose = null;
    private Interactable foamPistol = null;
    private PositionResetter playerPosResetter = null;
    public PositionResetter PlayerPosResetter { get => playerPosResetter; set => playerPosResetter = value; }
    public GameObject ExtinguisherSpawnPoint { get => extinguisherSpawnPoint; set => extinguisherSpawnPoint = value; }
    public GameObject Extinguisher2SpawnPoint { get => extinguisher2SpawnPoint; set => extinguisher2SpawnPoint = value; }

    private Text instructionsText;
    public Text InstructionsText { get => instructionsText; set => instructionsText = value; }
    private Text extinguisherText;
    public Text ExtinguisherText { get => extinguisherText; set => extinguisherText = value; }
    private Text endingText;
    public Text EndingText { get => endingText; set => endingText = value; }
    private Text temperatureText;
    public Text TemperatureText { get => temperatureText; set => temperatureText = value; }
    private Text timerText;
    public Text TimerText { get => timerText; set => timerText = value; }
    public enum task
    {
        Task1,
        Task2,
        Task3
    }

    public task CurrentTask;


    public void UpdateFeedbackCanvas()
    {
        switch (CurrentTask)
        {
            case task.Task1:
                feedbackText.text = FeedbackCanvas.GetComponent<VRTextBox>().TextCollection.Task1Text;
                foreach (Button button in buttons)
                {
                    button.gameObject.SetActive(false);
                }
                break;
            case task.Task2:
                feedbackText.text = FeedbackCanvas.GetComponent<VRTextBox>().TextCollection.Task2Text;
                foreach (Button button in buttons)
                {
                    button.gameObject.SetActive(false);
                }
                break;
            case task.Task3:
                feedbackText.text = FeedbackCanvas.GetComponent<VRTextBox>().TextCollection.Task3Text;
                foreach (Button button in buttons)
                {
                    button.gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    public void SetupTask()
    {
        switch (CurrentTask)
        {
            case task.Task1:
                UpdateFeedbackCanvas();
                HideInteractable(extinguisher);
                HideInteractable(extinguisher2);
                
                waterHose = Instantiate(waterHosePrefab, extinguisherSpawnPoint.transform.position, extinguisherPrefab.transform.rotation);
                instructionsText.text = instructionsText.GetComponentInParent<VRTextBox>().TextCollection.Task1Text;
                extinguisherText.text = extinguisherText.GetComponentInParent<VRTextBox>().TextCollection.Task1Text;
                

                temperatureText.text = "1330C (task1)";
                timerText.text = "You got 10s to finish task 1";
                if (playerPosResetter != null)
                {
                    playerPosResetter.ResetPosition();
                }
                //endingText.text = "This is a place for ending for Task 1";
                break;
            case task.Task2:
                UpdateFeedbackCanvas();
                HideInteractable(waterHose);
                foamPistol = Instantiate(foamPistolPrefab, extinguisherSpawnPoint.transform.position, extinguisherPrefab.transform.rotation);
                instructionsText.text = instructionsText.GetComponentInParent<VRTextBox>().TextCollection.Task2Text;
                extinguisherText.text = extinguisherText.GetComponentInParent<VRTextBox>().TextCollection.Task2Text;
              
                temperatureText.text = "1330C (task2)";
                timerText.text = "You got 10s to finish task 2";
                if (playerPosResetter != null)
                {
                    playerPosResetter.ResetPosition();
                }
                //endingText.text = "This is a place for ending for Task 2";
                break;
            case task.Task3:
                UpdateFeedbackCanvas();
                HideInteractable(foamPistol);
                HideInteractable(extinguisher);
                HideInteractable(extinguisher2);
                extinguisher = Instantiate(extinguisherPrefab, extinguisherSpawnPoint.transform.position, extinguisherPrefab.transform.rotation);
                extinguisher2 = Instantiate(extinguisher2Prefab, extinguisher2SpawnPoint.transform.position, extinguisher2Prefab.transform.rotation);
                instructionsText.text = instructionsText.GetComponentInParent<VRTextBox>().TextCollection.Task3Text;
                extinguisherText.text = extinguisherText.GetComponentInParent<VRTextBox>().TextCollection.Task3Text;
                
                temperatureText.text = "1330C (task3)";
                timerText.text = "You got 10s to finish task 3";
                if (playerPosResetter != null)
                {
                    playerPosResetter.ResetPosition();
                }
                //endingText.text = "This is a place for ending for Task 2";
                break;
            default:
                break;
        }
    }

    public void PickNextTask()
    {
        switch (CurrentTask)
        {
            case task.Task1:
                CurrentTask = task.Task2;
                break;
            case task.Task2:
                CurrentTask = task.Task3;
                break;
            case task.Task3:
                CurrentTask = task.Task1;
                break;
            default:
                break;
        }
    }

    public void SetupScenarioManager()
    {
        Debug.Log(feedbackCanvas.name);
        feedbackText = feedbackCanvas.GetComponentInChildren<Text>(true);
        buttons = feedbackCanvas.GetComponentsInChildren<Button>(true);
        SetupTask();               
    }

    private void HideInteractable(Interactable interactable)
    {
        if (interactable != null)
        {
            if (interactable.attachedToHand != null)
            {
                interactable.attachedToHand.DetachObject(interactable.gameObject);
            }
            interactable.gameObject.SetActive(false);
        }
    }

}
