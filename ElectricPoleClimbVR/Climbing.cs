using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Climbing : MonoBehaviour
{    
    public GameObject leftHand;                                                                                                    //left controller    
    public GameObject rightHand;                                                                                                   //right controller
    
    public GrabFeatures grabFeatures;       

    [SerializeField]
    private Rigidbody body;                                                                                                         //Players rigidbody

    public Vector3 prevPosLeft;                                                                                                     //Previous position of left controller
    public Vector3 prevPosRight;                                                                                                    //Previous position of rigth controller

    public SteamVR_Action_Single climbAction;
    //public SteamVR_ActionSet activateActionSetOnHover;                                                                              //List of input action sets

    public PlayerGuide playerGuide;


    // Start is called before the first frame update
    void Start()
    {
        prevPosLeft = new Vector3(body.transform.position.x, leftHand.transform.localPosition.y, 0);                                //Gets local y of left controller
        prevPosRight = new Vector3(body.transform.position.x, rightHand.transform.localPosition.y, 0);                              //Gets local y of right controller        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (PoleManager.instance == null)
            return;

        Vector3 polePosition = PoleManager.instance.pole.position;
        Vector3 leftPolePos = polePosition;
        Vector3 rightPolePos = polePosition;

        leftPolePos.y = leftHand.transform.position.y;
        rightPolePos.y = rightHand.transform.position.y;

        float leftDistance = Vector3.Distance(leftHand.transform.position, leftPolePos);
        float rightDistance = Vector3.Distance(rightHand.transform.position, rightPolePos);

        if (climbAction.GetAxis(SteamVR_Input_Sources.LeftHand) > 0.7f && leftDistance < 0.3f)                                            //If trigger on left controller is pulled
        {
            body.useGravity = false;                                                                                                //Disable players gravity
            body.isKinematic = true;                                                                                                //Make player kinematic            
            transform.position += (prevPosLeft - new Vector3(body.transform.position.x, leftHand.transform.localPosition.y, 0));    //Move player to opposite direction of left hand movement on y-axis            
        }

        else if (climbAction.GetAxis(SteamVR_Input_Sources.RightHand) > 0.7f && rightDistance < 0.3f)                                      //If trigger on right controller is pulled
        {
            body.useGravity = false;                                                                                                //Disable players gravity
            body.isKinematic = true;                                                                                                //Make player kinematic                       
            transform.position += (prevPosRight - new Vector3(body.transform.position.x, rightHand.transform.localPosition.y, 0));  //Move player to opposite direction of right hand movement on y-axis            
        }

        if (transform.position.y > 9)
        {
            transform.position = new Vector3(transform.position.x, 9, transform.position.z);
        }

        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

        prevPosLeft = new Vector3(body.transform.position.x, leftHand.transform.localPosition.y, 0);                                //Gets local y of left controller
        prevPosRight = new Vector3(body.transform.position.x, rightHand.transform.localPosition.y, 0);                              //Gets local y of right controller

    }

    public void ActivateClimb()                                                                                                     //Activates custom input system
    {
        //if (!grabFeatures.objectInHand && (playerGuide.playerProgress == 3 || playerGuide.playerProgress == 9))
        {
            //activateActionSetOnHover.Activate();
        }
    }


    public void DeactivateClimb()                                                                                                   //Deactivates custom input system
    {
        //activateActionSetOnHover.Deactivate();
    }
}
