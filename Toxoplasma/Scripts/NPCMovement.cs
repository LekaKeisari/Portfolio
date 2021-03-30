using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{

    private GameManager gameManager;

    public bool npcMovementEnabled = false;

    private Rigidbody rb;

    private Animator animator;

    [SerializeField]
    private float movementSpeed = 4;

    [SerializeField]
    private float movementSmoothing = 1;

    [SerializeField]
    private float stopSpeed = .1f;

    [SerializeField]
    private float distanceToPlayer = 3f;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        gameManager.nPC = gameObject;


        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        StartCoroutine(AssignPlayer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (npcMovementEnabled)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > distanceToPlayer)
            {
                Move(player.transform.position - transform.position);
                StartCoroutine(MoveWithDelay(true));
            }
            else
            {
                StopMoving();
                StartCoroutine(MoveWithDelay(false));
            }
        }
        else
        {
            StopMoving();
            StartCoroutine(MoveWithDelay(false));
        }
    }

    private void Move(Vector3 direction)
    {
        //if (animator)
        //{
        //    animator.SetBool("isRunning", true);
        //}

        rb.isKinematic = false;

        rb.velocity = direction * movementSpeed;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), movementSmoothing);

    }

    private void StopMoving()
    {
        //if (animator && rb.velocity.magnitude < 0.01f)
        //{
        //    animator.SetBool("isRunning", false);
        //}

        rb.velocity -= new Vector3(rb.velocity.x, 0, rb.velocity.z) * stopSpeed;
        rb.freezeRotation = true;

        rb.isKinematic = true;
    }

    private IEnumerator MoveWithDelay(bool startMoving)
    {
        if (startMoving)
        {
            yield return new WaitForSeconds(.01f);
            if (animator)
            {
                animator.SetBool("isRunning", true);
            }
        }

        if (!startMoving)
        {
            yield return new WaitForSeconds(.5f);
            if (animator && rb.velocity.magnitude < 0.01f)
            {
                animator.SetBool("isRunning", false);
            }
        }
    }

    private IEnumerator AssignPlayer()
    {
        while (player == null)
        {
            player = gameManager.player;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
