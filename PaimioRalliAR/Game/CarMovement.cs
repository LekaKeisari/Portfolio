using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Movement variables:")]

    [SerializeField]
    private float sidewaysMoveSpeed = 10;

    private float horizontalMove = 0f;          // Variable to assist in sidewaysmovement smoothing
    private Vector3 carVelocity = Vector3.zero; // Variable to assist in sidewaysmovement smoothing

    [SerializeField]
    private float sidewaysScaling = 20;         // How much car velocity affects sideways movement.

    [SerializeField]
    private float sidewaysBottomCap = 0;        // Minimum sidewaysmovementspeed

    [Range(0, .3f)]
    [SerializeField]
    private float movementSmoothing = .01f;     // How much to smooth out the movement

    [Range(0, 1f)]
    [SerializeField]
    private float jumpSmoothingLow = .1f;         // How much to smooth rotation on jumps


    [Range(0, 1f)]
    [SerializeField]
    private float jumpSmoothingMid = .2f;

    [Range(0, 1f)]
    [SerializeField]
    private float jumpSmoothingHigh = .2f;

    private float jumpSmoothing;

    [SerializeField]
    private float maxHeight = 10;               // At what height the game pushes car back down

    [Header("Prefabs:")]

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Transform centerOfMass;

    [SerializeField]
    private CameraMovement cameraMovement;

    [Header("Particle Systems:")]

    [SerializeField]
    private GameObject windParticleSystem;
 
    public ParticleSystem grassParticleSystemLeft;       
    public ParticleSystem grassParticleSystemRight;

    public GameObject tireGrassParSysLeft;
    public GameObject tireGrassParSysRight;

    public GameObject tireGravelParSysLeft;
    public GameObject tireGravelParSysRight;

    [Header("Info for Debugging:")]

    public bool isJumping = false;
    public bool jumpIsHigh = false;
    public bool moveAllowed = false;
    
    public float boostEndTime;                  // When it is time to stop boost mode
    public bool boosting = false;    
    
    public float velocity;
    public float accelerationForce;

    private float ScreenWidth;                  // Device screenwidth for mobile controls
    private CameraShake cameraShake;
    private GameUIManager gameUIManager;
    private Animator animator;

    private bool forceZero = false;

    private void Start()
    {
        ScreenWidth = Screen.width;                                                         // Detects screenwidth, used in mobile controls
        animator = this.GetComponent<Animator>();
        rb.centerOfMass = centerOfMass.localPosition;                                       // Sets players center of mass to location of game object called center of mass
        accelerationForce = GameManager.instance.acceleration;                              // Get acceleration value from GameManager
        cameraShake = GameObjectManager.instance.allObjects[6].GetComponent<CameraShake>(); // Get CameraShake from GameObjectManagers list
        gameUIManager = GameObjectManager.instance.allObjects[2].GetComponent<GameUIManager>();
    }

    private void Update()
    {

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR                                     // Controls for editor
        if (velocity > 0.1f)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * sidewaysMoveSpeed;                // Get user input and multiply with sidewaysmovespeed variable
            if (Input.GetAxisRaw("Horizontal") < 0 && !isJumping)                               // If player input orders move left and grounded -> toggle animations accordingly
            {
                animator.SetBool("turnRight", false);
                animator.SetBool("turnLeft", true);
            }
            if (Input.GetAxisRaw("Horizontal") > 0 && !isJumping)                               // If player input orders move right and grounded -> toggle animations accordingly
            {
                animator.SetBool("turnRight", true);
                animator.SetBool("turnLeft", false);
            }
            if (Input.GetAxisRaw("Horizontal") == 0 && !isJumping)                              // If no player input and grounded -> toggle animations accordingly
            {
                animator.SetBool("turnRight", false);
                animator.SetBool("turnLeft", false);
            }
        }
#else                                                                                       //Controls for mobile devices

        int i = 0;

        //loop over every touch found
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).position.x > ScreenWidth / 2 /*&& !isJumping*/ && velocity > 0.1f)               //If touch is detected in right side of the screen and grounded
            {
                //move right and toggle animations accordingly
                horizontalMove = sidewaysMoveSpeed;
                animator.SetBool("turnRight", true);
                animator.SetBool("turnLeft", false);
            }    
            if (Input.GetTouch(i).position.x < ScreenWidth / 2 /*&& !isJumping*/ && velocity > 0.1f)               //If touch is detected in left side of the screen and grounded
            {
                //move left and toggle animations accordingly
                horizontalMove = -sidewaysMoveSpeed;
                animator.SetBool("turnRight", false);
                animator.SetBool("turnLeft", true);
            
            }
            ++i;
        }
        if(Input.touchCount == 0 /*&& !isJumping*/)                                             //If no input and grounded
        {
            //move straight and toggle animations accordingly
            horizontalMove = 0f;
            animator.SetBool("turnRight", false);
            animator.SetBool("turnLeft", false);
        }
        

#endif

    }

    void FixedUpdate()
    {
        velocity = rb.velocity.z;                                               //Set velocity variable as players velocity vectors z-axis
        Vector3 down = transform.TransformDirection(Vector3.down);              //Set vector pointing straight down from current position

        sidewaysMoveSpeed = velocity / sidewaysScaling + sidewaysBottomCap;     //Set sidewaysmovespeed according to current velocity

        if (GameManager.instance.batteryLife > 0 && forceZero == true)
        {
            accelerationForce = GameManager.instance.Acceleration;
            forceZero = false;
        }

        if (GameManager.instance.batteryLife <= 0 && forceZero == false)        //If battery is dead
        {
            accelerationForce = 0F;                                             //Set acceleration to 0
            forceZero = true;
        }

        if (Mathf.Abs(transform.position.x) > 10)                               //If player tries to move too much sideways
        {
            Vector3 backTocenter = new Vector3(-transform.position.x, 0, 0);    //Set vector pointing toward the center of track
            rb.AddForce(backTocenter * 30, ForceMode.Impulse);                  //Push player back towards center
        }
        else
        {
        Move(horizontalMove * Time.fixedDeltaTime);                             //Move sideways with smoothing

        }

        if (transform.position.y > maxHeight)                                   //If player jumps higher than set maximum height
        {
            if (transform.position.y > maxHeight + 3)
            {
                jumpIsHigh = true;                                                  //Set bool to indicate jump is high
            }
            Vector3 backToGround = new Vector3(0, -transform.position.y, 0);    //Set vector pointing back down
            rb.AddForce(backToGround * 40, ForceMode.Impulse);                  //Push player back down
        }

        if (Physics.Raycast(transform.position, down, 1f))                      //If player is close to the ground
        {           
            if (velocity <= GameManager.instance.maxSpeed && moveAllowed)       //If velocity is lower than maxSpeed and movement is allowed
            {
                rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration); //Add acceleration force forward
            }



            if (GameManager.instance.velocity <= 35)
            {
                jumpSmoothing = jumpSmoothingLow;
                rb.angularDrag = 2;
            }
            else if (GameManager.instance.velocity > 35 && GameManager.instance.velocity <= 45)
            {
                jumpSmoothing = jumpSmoothingMid;
                rb.angularDrag = 2;
            }
            else
            {
                jumpSmoothing = jumpSmoothingHigh;
                rb.angularDrag = 0.05f;
            }
            //Debug.Log("Jump: " + jumpSmoothing);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, jumpSmoothing);   //Rotate player towards normal with smoothing           
        }

        if (Physics.Raycast(transform.position, down, 0.4f) && Mathf.Abs(transform.position.x) < 10)                    //If player is touching ground
        {
            isJumping = false;                                                  //Sets bool to indicate player is not jumping
            if (GameManager.instance.velocity > 10)
            {
                tireGravelParSysLeft.SetActive(true);                               //Activate proper particle system
                tireGravelParSysRight.SetActive(true);                              //Activate proper particle system
            }
            else
            {
                tireGravelParSysLeft.SetActive(false);
                tireGravelParSysRight.SetActive(false);
            }
        }

        else                                                                    //If player is not touching ground
        {
            isJumping = true;                                                   //Set bool to indicate player is jumping
            tireGravelParSysLeft.SetActive(false);                              //Activate proper particle system
            tireGravelParSysRight.SetActive(false);                             //Activate proper particle system
        }

        if (GameManager.instance.boostMode)                                     //If boostmode is activated
        {
            BoostModeHandler();                                                 //Call method for boost mode
        }
    }

    public void Move(float move)                                                //Move player with smoothing
    {
        if (!GameManager.instance.gameOver && moveAllowed)                      //Check that game is not over and movement is allowed
        {            
            Vector3 targetVelocity = new Vector3(move * 10f, rb.velocity.y,rb.velocity.z);                      // Move the character by finding the target velocity            
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref carVelocity, movementSmoothing);  // And then smoothing it out and applying it to the character
        }
    }

    public void ObstacleCollision(float obstacleEffect)                                 //Handles obstacles depending on obstacles effect
    {
        Vector3 effectDirection = new Vector3(0, 0, -obstacleEffect * rb.velocity.z);   //Set effect direction towards player on z-axis

        rb.AddForce(effectDirection * 100, ForceMode.Impulse);                          //Add impulse force against player

        StartCoroutine(cameraShake.ShakeCamera(true, true, false, .25f, .25f));         //Shake camera
        GameSoundManager.instance.PlayCrashSound();
    }

    public void BoostModeHandler()
    {
        if (GameManager.instance.boostMode && boostEndTime > Time.time && !boosting)    //If boostmode is on and boost has still time left and player is not yet boosting
        {
            accelerationForce *= GameManager.instance.speedBoost;                       //Multiply acceleration with speed boost value
            GameManager.instance.maxSpeed *= GameManager.instance.speedBoost;           //Multiply maxSpeed with speed boost value
            boosting = true;                                                            //Set bool to indicate player is now boosting

            if (GameManager.instance.velocity >= 40)                                           //If particlesystem is not playing
            {
                windParticleSystem.SetActive(true);                                              //Play particle system
            }
            StartCoroutine(cameraShake.ShakeCamera(false, true, true, .5f, .5f));       //Shake camera
            StartCoroutine(cameraMovement.MoveCameraZAxis(GameManager.instance.speedBoostDuration -1f));    //More boost mode camera effects
            gameUIManager.HighlightScoreOn();
        }

        if (GameManager.instance.boostMode && boostEndTime < Time.time && boosting)     //If boostmode is on and boost has no time left and player is boosting
        {
            accelerationForce /= GameManager.instance.speedBoost;                       //Divide acceleration with speedboost value
            GameManager.instance.maxSpeed /= GameManager.instance.speedBoost;           //Divide maxSpeed with speedboost value
            GameManager.instance.boostMode = false;                                     //Set bool to indicate boostmode has ended
            boosting = false;                                                           //Set bool to indicate player is no longer boosting
            windParticleSystem.SetActive(false);                                                  //Stop particle system
            StartCoroutine(cameraMovement.MoveCameraZAxis(GameManager.instance.speedBoostDuration));        //Camera effects for boost modes ending
            gameUIManager.HighlightScoreOff();
        }

        if (GameManager.instance.velocity < 40)
        {
                windParticleSystem.SetActive(false);
        }
    }
}
