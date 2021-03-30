using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    private GameManager gameManager;
    public GameObject climbTarget;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (tag)
            {
                case "Climb":
                    gameManager.activeIntreractable = this;
                    gameManager.ableToClimb = true;
                    
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (tag)
            {
                case "Climb":
                    gameManager.ableToClimb = false;
                    break;
                default:
                    break;
            }
        }
    }

    public void Climb()
    {
        Debug.Log("climb");
        gameManager.player.transform.position = climbTarget.transform.position;
    }
}
