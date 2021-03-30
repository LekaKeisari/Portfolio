using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;
//public class FireVFXEmitter
//{
//    public ParticleSystem.EmissionModule emitter;
//    public float amountOfParticles;

//    public FireVFXEmitter(ParticleSystem.EmissionModule e, float a)
//    {
//        emitter = e;
//        amountOfParticles = a;
//    }
//}

public class FireVFX : MonoBehaviour
{
    [SerializeField] private Text temperatureText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text feedbackText;
    [SerializeField] ScenarioManagerSO scenarioManager;
    [SerializeField] TeleportPoint tpPoint;
    [SerializeField] AudioSource audioSource;
    private CapsuleCollider fireCollider;
    [Header("Fire Stats")]
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private float recoverRate = 10f;

    [SerializeField] private VisualEffect fireEffect;

    [Header("Events")]
    public UnityEvent onFireExtinguished;

    private bool isCounting = false;
    private bool isTimeOut = false;
    private bool fireAlive;
    private float currentHealth;
    private float defaultAmountOfParticles;
    private float defaultRadius;
    private bool underControl;
    private Vector3 windForceDirection = new Vector3(0, 0, 1);

    void Start()
    {
        defaultAmountOfParticles = fireEffect.GetFloat("ParticleAmount");
        defaultRadius = fireEffect.GetFloat("Radius");
        fireCollider = GetComponent<CapsuleCollider>();
        StartFire();
    }

    public void StartFire()
    {
        if (fireEffect == null || fireAlive == true)
            return;

        fireAlive = true;
        StartCoroutine(FireRecoverLoop());
        fireEffect.SetFloat("ParticleAmount", defaultAmountOfParticles);
        fireEffect.SetFloat("Radius", defaultRadius);
        fireEffect.Play();

        currentHealth = startHealth;

    }

    void StopFire()
    {
        if (fireEffect == null || fireAlive == false)
            return;

        fireAlive = false;
        fireEffect.Stop();

        if (onFireExtinguished != null)
            onFireExtinguished.Invoke();
        StopAllCoroutines();
        audioSource.Stop();
    }

    void UpdateParticleAmounts()
    {

        fireEffect.SetFloat("ParticleAmount", (currentHealth / startHealth) * defaultAmountOfParticles);
        temperatureText.text = string.Format("{0:0}", currentHealth * 13f) + "C";
    }

    public void ReduceFireHealth(float healthReduction)
    {
        if (fireEffect == null || fireAlive == false || isTimeOut)
            return;
        switch (scenarioManager.CurrentTask)
        {
            case ScenarioManagerSO.task.Task1:
                currentHealth -= healthReduction / 6;
                UpdateParticleAmounts();
                DecreaseRadius(healthReduction);
                break;
            case ScenarioManagerSO.task.Task2:
                currentHealth -= healthReduction / 7.5f;
                UpdateParticleAmounts();
                break;
            case ScenarioManagerSO.task.Task3:
                currentHealth -= healthReduction;
                UpdateParticleAmounts();
                break;
        }
        //currentHealth -= healthReduction;
        //UpdateParticleAmounts();

        if (currentHealth < 0f)
        {
            currentHealth = 0f;
            StopFire();
        }
    }
    void DecreaseRadius(float rate)
    {
        float currentRadius = fireEffect.GetFloat("Radius");
        fireEffect.SetFloat("Radius", currentRadius - rate / 5000);
    }
    public void StartTimer()
    {
        if (isCounting || fireAlive == false)
            return;
        else
        {
            StartCoroutine(Timer(10));
        }
    }
    public void ResetTimer()
    {
        isCounting = false;
        isTimeOut = false;
    }
    public void DecreaseWindForce(float rate)
    {
        if (fireEffect == null || fireAlive == false || !isTimeOut || underControl)
            return;
        DecreaseRadius(1);
        Vector3 currentWindForce = fireEffect.GetVector3("WindForce1");
        if (currentWindForce == Vector3.zero)
        {
            underControl = true;
            scenarioManager.UpdateFeedbackCanvas();
            //tpPoint.SetLocked(false);
            //scenarioManager.PickNextTask();
            StartCoroutine(scenarioManager.FeedbackCanvas.GetComponent<UIController>().StartNextTask());
        }

        if (currentWindForce.z > 0 || currentWindForce.x > 0 || currentWindForce.y > 0)
        {
            fireEffect.SetVector3("WindForce1", currentWindForce - (windForceDirection * rate));
            fireCollider.center -= windForceDirection * rate * 3.5f;
            fireCollider.height -= rate * 10;
        }

        currentWindForce = fireEffect.GetVector3("WindForce1");
        if (currentWindForce.x < 0)
            currentWindForce.x = 0;
        if (currentWindForce.y < 0)
            currentWindForce.y = 0;
        if (currentWindForce.z < 0)
            currentWindForce.z = 0;
        fireEffect.SetVector3("WindForce1", currentWindForce);
        if (fireCollider.center.x < 0 || fireCollider.center.y < 0 || fireCollider.center.z < 0)
            fireCollider.center = Vector3.zero;
        if (fireCollider.height < 0)
            fireCollider.height = 0;

    }
    void TimeOutFeedbackTextUpdate()
    {
        if (scenarioManager.FeedbackCanvas.GetComponent<VRTextBox>().TimeOutTextCollection != null)
        {
            switch (scenarioManager.CurrentTask)
            {
                case ScenarioManagerSO.task.Task1:
                    scenarioManager.FeedbackText.text = scenarioManager.FeedbackCanvas.GetComponent<VRTextBox>().TimeOutTextCollection.Task1Text;
                    scenarioManager.FeedbackCanvas.gameObject.SetActive(true);
                    AddWindForce(windForceDirection);
                    break;
                case ScenarioManagerSO.task.Task2:
                    scenarioManager.FeedbackText.text = scenarioManager.FeedbackCanvas.GetComponent<VRTextBox>().TimeOutTextCollection.Task2Text;
                    scenarioManager.FeedbackCanvas.gameObject.SetActive(true);
                    //tpPoint.SetLocked(false);
                    //scenarioManager.PickNextTask();
                    StartCoroutine(scenarioManager.FeedbackCanvas.GetComponent<UIController>().StartNextTask());
                    break;
                case ScenarioManagerSO.task.Task3:
                    scenarioManager.FeedbackText.text = scenarioManager.FeedbackCanvas.GetComponent<VRTextBox>().TimeOutTextCollection.Task3Text;
                    scenarioManager.FeedbackCanvas.gameObject.SetActive(true);
                    //tpPoint.SetLocked(false);
                    //scenarioManager.PickNextTask();
                    //StartCoroutine(scenarioManager.FeedbackCanvas.GetComponent<UIController>().StartNextTask());
                    break;
            }
        }
        else
        {
            scenarioManager.FeedbackText.text = "TimeOutTextCollection is missing from FeedbackText";
        }

    }
    IEnumerator Timer(int duration)
    {
        while (duration > 0)
        {
            isCounting = true;
            timerText.text = duration.ToString();
            duration--;
            yield return new WaitForSeconds(1);
        }
        timerText.text = "Time out";
        isTimeOut = true;
        TimeOutFeedbackTextUpdate();
    }
    IEnumerator FireRecoverLoop()
    {
        while (fireAlive == true)
        {
            if (currentHealth < startHealth)
            {
                currentHealth += recoverRate * Time.deltaTime;

                if (currentHealth > startHealth)
                    currentHealth = startHealth;
            }
            UpdateParticleAmounts();
            yield return null;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddWindForce(new Vector3(0, 0, 1));
        }
    }
    void AddWindForce(Vector3 direction)
    {
        underControl = false;
        windForceDirection = direction;
        fireCollider.center = direction * 3.5f;
        fireCollider.height = 10;
        fireEffect.SetVector3("WindForce1", direction);
    }



}
