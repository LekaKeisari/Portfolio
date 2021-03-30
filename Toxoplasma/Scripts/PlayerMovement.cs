using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    public GameObject cameraTarget;

    [SerializeField]
    private float movementSpeed = 1f;

    [SerializeField]
    private float stopSpeed = 0.1f;

    [SerializeField]
    private float movementSmoothing = 0.2f;
    [SerializeField]
    private float maxSpeed = 5;

    private bool interactableNear = false;

    private GameManager gameManager;

    [HideInInspector]
    public Animator animator;

    private Vector3 moveDirection = Vector3.zero;

    private bool isGrounded = false;

    [SerializeField]
    private float groundDistance = 1.05f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        gameManager.player = gameObject;

        gameManager.restarted = false;

        Cursor.visible = false;

        gameManager.CallSetupEventsCoroutine();

        animator = GetComponentInChildren<Animator>();

        if (gameManager.playerPosition != Vector3.zero)
        {
            transform.position = gameManager.playerPosition;
            transform.rotation = gameManager.playerRotation;

            if (gameManager.cameraFollowDisabled)
            {
                cameraTarget.transform.position = gameManager.cameraTargetPosition;
                cameraTarget.transform.rotation = gameManager.cameraTargetRotation;
                cameraTarget.GetComponentInChildren<CameraMovement>().transform.localPosition = gameManager.mainCameraOffset;
                cameraTarget.GetComponentInChildren<CameraMovement>().transform.localRotation = gameManager.cameraRotation;

            }
            else
            {
            cameraTarget.transform.rotation = gameManager.cameraTargetRotation;
            cameraTarget.transform.position = gameManager.playerPosition;
            cameraTarget.GetComponentInChildren<CameraMovement>().transform.localPosition = gameManager.mainCameraOffset;
            cameraTarget.GetComponentInChildren<CameraMovement>().transform.localRotation = gameManager.cameraRotation;

            }
        }
        rb = GetComponent<Rigidbody>();        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, groundDistance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && !gameManager.moveEventInProgress)
        {
            StopMoving();
        }
        if (gameManager.playerMoveAllowed && gameManager.dialogueManager.dialogueDone && isGrounded)
        {
            moveDirection = cameraTarget.transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (moveDirection != Vector3.zero)
            {
                Move(moveDirection);
            }            
        }               
    }

    public void Move(Vector3 direction)
    {
        animator.SetBool("isRunning", true);

        rb.velocity = direction * movementSpeed;
                
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up) , movementSmoothing);
        
    }

    private void StopMoving()
    {
        animator.SetBool("isRunning", false);
        rb.velocity -= new Vector3(rb.velocity.x, 0, rb.velocity.z) * stopSpeed;
        rb.freezeRotation = true;
    }
}
