using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class FollowHand : MonoBehaviour
{
    private GameObject leftHand;
    private GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        leftHand = Player.instance.GetComponent<Climbing>().leftHand;
        rightHand = Player.instance.GetComponent<Climbing>().rightHand;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (tag)
        {
            case "leftHandCanvas":
                transform.position = Vector3.Lerp(transform.position, new Vector3(leftHand.transform.position.x, leftHand.transform.position.y + 0.3f, leftHand.transform.position.z), 0.5f);
                //transform.rotation = Quaternion.Lerp(transform.rotation, leftHand.transform.rotation, 0.1f);
                transform.LookAt(Camera.main.transform);
                break;

            case "rightHandCanvas":
                transform.position = Vector3.Lerp(transform.position, new Vector3(rightHand.transform.position.x, rightHand.transform.position.y + 0.2f, rightHand.transform.position.z), 0.5f);
                //transform.rotation = Quaternion.Lerp(transform.rotation, rightHand.transform.rotation, 0.1f);
                transform.LookAt(Camera.main.transform);
                break;

            default:
                break;
        }
    }
}
