using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRTextBox : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private ScenarioManagerSO scenarioManagerSO;
    [SerializeField] private TextCollectionSO textCollection;
    public TextCollectionSO TextCollection { get => textCollection; }

    [SerializeField] private TextCollectionSO timeOutTextCollection;
    public TextCollectionSO TimeOutTextCollection { get => timeOutTextCollection; }

    private Text vrText;
    public Text VrText { get => vrText; set => vrText = value; }

    public enum TextType
    {
        Instruction,
        ExtinguisherDescription,
        Temperature,
        Timer,
        Feedback
    }

    public TextType textType;

    private void Awake()
    {
        VrText = GetComponentInChildren<Text>();

        switch (textType)
        {
            case TextType.Instruction:
                scenarioManagerSO.InstructionsText = VrText;
                break;
            case TextType.ExtinguisherDescription:
                scenarioManagerSO.ExtinguisherText = VrText;
                break;
            case TextType.Temperature:
                scenarioManagerSO.TemperatureText = VrText;
                break;
            case TextType.Timer:
                scenarioManagerSO.TimerText = VrText;
                break;
            case TextType.Feedback:
                scenarioManagerSO.EndingText = VrText;
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(mainCam.transform);
    }
    public void SetCanvasActive(bool value)
    {
        this.gameObject.SetActive(value);
    }
}
