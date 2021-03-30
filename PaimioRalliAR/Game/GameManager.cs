using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private GameUIManager gameUIManager;
    private GameObject player;
    private CarMovement carMovement;
    public ObjectPool Pool { get; set; }
    [SerializeField] GameObject tutorial;
    [SerializeField] GameObject countdown;
    [SerializeField] GameObject arrows;

    List<ParticleSystem> activeParticles = new List<ParticleSystem>();

    [Header("Gameplay variables:")]

    public float acceleration = 20f;                    //Cars acceleration
    public float accelerationMultiplier = 1f;           //Multiplier for acceleration to be used in car upgrades, higher for better upgrades

    public float maxSpeed = 1f;                         //Maximum speed for movement
    public float maxSpeedMultiplier = 1f;               //Multiplier to be used in car upgrades, higher for better upgrades

    public float batteryDrain = 0.1f;                   //How fast battery drains
    public float batteryLife = 1;                       //Variable for current batterylife
    public float maxBatteryLife = 1;                    //Maximum batterylife

    public float obstacleMultiplier = 1f;               //Multiplier to define how much effect the obstacle has on player. Lower for better upgrades

    public float velocity = 0f;                         //How fast car is moving

    [Header("Obstacles and boosts:")]

    public float speedReduction = 0.1f;                 //How much speed obstacles reduce
    public float batteryBoost = 0.1f;                   //How much batterylife boosts give
    public float speedBoost = 2f;                       //How much speed boosts give
    public float speedBoostDuration = 2f;               //How long speed boost lasts
    public float grassEffect = 0.1f;                    //How much grass slows player down

    private float playerScore;                          

    private float rescueTimer;                          //Timer to determine if rescue is needed
    private float startRescueAfterSeconds = 2f;         //How long game waits before rescing player
    private bool rescueNeeded = false;                  //Bool that tells if rescue is needed

    private float gameOverTimer;                        //Timer to determine if game is over
    private float gameOverAfterSeconds = 2f;            //How long game waits before game over is called

    [Header("Bools for Debugging:")]
    public bool gameOver = false;                       //Bool to determine if game is over
    public bool gameOverPending = false;                //Bool to determine is game about to be over

    public bool setupComplete = false;                  //Bool to determine if initial setup is complete
    public bool boostMode = false;                      //Bool to determine if boost mode is currently on

    private bool startCounterFinished = false;          //Bool to determine if race start counter is finished

    public float PlayerScore                            //Property for player score that updates score text
    {
        get
        {
            return playerScore;
        }

        set
        {
            this.playerScore = Mathf.Round(value) -7;
            gameUIManager.playerScoreTxt.text = "Score: " + this.playerScore.ToString("0");
        }
    }

    public float Acceleration                           //Property for acceleration that updates cars acceleration force when value is changed
    {
        get
        {
            return acceleration;
        }

        set
        {
            this.acceleration = value;
            //carMovement.accelerationForce = value;
        }
    }

    void Awake()
    {
        //If this script does not exit already, use this current instance
        if (instance == null)
            instance = this;

        //If this script already exit, DESTROY this current instance
        else if (instance != this)
            Destroy(gameObject);

        SetStats();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameUIManager = GameObjectManager.instance.allObjects[2].GetComponent<GameUIManager>();         //Get GameUIManager from singleton
        player = GameObjectManager.instance.allObjects[0];                                              //Get Player from singleton
        carMovement = player.GetComponent<CarMovement>();

        Pool = GetComponent<ObjectPool>();

        if (StaticVariables.carTutDone)
        {
            StartCoroutine(RaceStart());                                                                    //Start race start countdown
        }
        else
        {
            tutorial.SetActive(true);
        }

    }


    private void Update()
    {

        PlayerScore = player.transform.position.z;                                                      //Player score is equal to moved distance on z-axis   

        velocity = carMovement.velocity;                                                                //Get players velocity

        if (batteryLife == 0 && carMovement.velocity < 0.1f && !gameOverPending)                        //If battery is dead, speed is almost 0 and game is not yet about to be over
        {
            gameOverTimer = Time.time + gameOverAfterSeconds;                                           //Calculate game over timer with current time + set variable
            gameOverPending = true;                                                                     //Game is about to be over
        }

        if (gameOverPending == true && batteryLife > 0)
        {
            gameOverPending = false;
        }

        if (Time.time > gameOverTimer && gameOverPending && batteryLife == 0)                           //If game over timer has ended and game is about to be over
        {
            gameUIManager.GameOver();                                                                   //Call game over
        }            

        if (batteryLife > 0 && carMovement.velocity < 0.5f && !rescueNeeded && carMovement.moveAllowed) //If player is stuck
        {
            rescueTimer = Time.time + startRescueAfterSeconds;                                          //Start rescue timer
            rescueNeeded = true;                                                                        
        }

        if (Time.time > rescueTimer && rescueNeeded)                                                    //If rescue timer is up and rescue is still needed
        {
            RescuePlayer();                                                                             //Call rescue player function
            rescueNeeded = false;                                                                       //rescue is no longer needed
        }

        if (batteryLife > 0 && carMovement.velocity > 0.5f && rescueNeeded)                             //If player is no longer stuck
        {
            rescueNeeded = false;                                                                       //Set rescue needed to false
        }
        //Debug.Log("acceleration: " + Acceleration);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (batteryLife > 0 && carMovement.moveAllowed)
        {
            batteryLife -= batteryDrain;                                                                //Drains battery
        }

        if (batteryLife <= 0)                                                                           //Forces batterylife to 0 if gone under 0
        {
            batteryLife = 0;
        }
    }


    //Spawns particles to player position and adds them to object pool
    public void spawnParticles(string type)                                                             //Spawns particle effects by name
    {
        ParticleSystem particleSystem = Pool.SpawnPoolObject(type).GetComponent<ParticleSystem>();            //Gets particle system from object pool

        particleSystem.transform.position = player.transform.position;                                  //Spawns particle system at player position
        particleSystem.transform.SetParent(player.transform);                                           //Sets player as particle systems parent

        //activeParticles.Add(particleSystem);                                                            //Add spawned particle system to a list
        Destroy(particleSystem.gameObject, particleSystem.main.duration);
    }

    private void RescuePlayer()                                                                         //Function to rescue player if stuck
    {                                                        
        player.transform.position = new Vector3(0, player.transform.position.y + 5f, player.transform.position.z + 10f);    //Moves player to center, a little bit up and forward
        player.transform.rotation = Quaternion.identity;                                                                    //Change player rotation to default
    }

    private IEnumerator RaceStart()                                                                     //Coroutine for race start countdown
    {

        countdown.SetActive(true);

        gameUIManager.startCounterTxt.text = "";

        yield return new WaitForSeconds(2f);
        gameUIManager.startCounterTxt.text = "3";
        GameSoundManager.instance.PlayCountDown();
        
        yield return new WaitForSeconds(1f);
        gameUIManager.startCounterTxt.text = "2";
        GameSoundManager.instance.PlayCountDown();

        yield return new WaitForSeconds(1f);
        gameUIManager.startCounterTxt.text = "1";
        GameSoundManager.instance.PlayCountDown();

        yield return new WaitForSeconds(1f);

        gameUIManager.startCounterTxt.text = "Go!";
        arrows.SetActive(true);
        GameSoundManager.instance.PlayCountDown(true);
        carMovement.moveAllowed = true;

        yield return new WaitForSeconds(1f);

        gameUIManager.startCounterTxt.text = "";
    }


    //Sets up variables with multiplier effects on start up
    private void SetStats()
    {
        Acceleration = StaticVariables.Acceleration;
        accelerationMultiplier = StaticVariables.AccelerationMultiplier;
        maxSpeed = StaticVariables.MaxSpeed;
        maxSpeedMultiplier = StaticVariables.MaxSpeedMultiplier;
        maxBatteryLife = StaticVariables.BatteryLife * StaticVariables.BatteryLifeMultiplier;
        obstacleMultiplier = StaticVariables.Armour * StaticVariables.ArmourMultiplier;

        Acceleration *= accelerationMultiplier;
        maxSpeed *= maxSpeedMultiplier;
        batteryLife = maxBatteryLife;
        speedReduction = speedReduction / obstacleMultiplier;
    }

    public void SetCarTutDone()
    {
        StaticVariables.carTutDone = true;
        StartCoroutine(RaceStart());
    }
}
