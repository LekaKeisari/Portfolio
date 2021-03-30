using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class Snapper : MonoBehaviour
{
    [Tooltip("Drag in the GameObject you wish to snap with")]
    public GameObject snapTarget;

    [Header("Snapping offsets:")]
    [Tooltip("Offset from center of the snapTarget. Determines where this object snaps.")]
    [SerializeField]
    private float offsetX = 0;
    [SerializeField]
    private float offsetY = 0.2f;
    [SerializeField]
    private float offsetZ = 0;

    public bool isSnapped = false;                          //Bool to determine if object is already snapped in place

    public UnityEvent onSnapped;

    private Rigidbody rb;                                   //This objects rigidbody
    private float startTime;                                //Time when objects starts snapping
    private float speed = 0.01f;                            //Objects movement speed when snapping

    private Vector3 snapTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                     //Gets this objects rigidbody
    }

    private void OnTriggerEnter(Collider other)             
    {
        if (other.tag == "baseCube")
        {
            startTime = Time.time;                          //Gets time when object enters marks collider
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "baseCube")                                                                                                                                        //Tag is used to identify the correct object to snap with
        {
            snapTargetPos = new Vector3(snapTarget.transform.position.x + offsetX, snapTarget.transform.position.y + offsetY, snapTarget.transform.position.z + offsetZ);

            float journeyLength = Vector3.Distance(transform.position, snapTargetPos);                                                                                      //Distance from hand to desired snap position
                        
            float distanceCovered = (Time.time - startTime) * speed;                                                                                                        //Distance already moved
            float fractionOfJourney = distanceCovered / journeyLength;                                                                                                      //Fraction of travelled distance and total distance

            transform.position = Vector3.Lerp(transform.position, snapTargetPos, fractionOfJourney);                                                                        //Moves the object smoothly to its destination
            transform.rotation = Quaternion.Lerp(transform.rotation, snapTarget.transform.rotation, fractionOfJourney);                                                     //Rotates smoothly to align with destination

            isSnapped = true;
            MakeThisKinematic();
        }

        if (transform.position == snapTargetPos && isSnapped)
        {
            //transform.SetParent(snapTarget.transform);

            if (onSnapped != null)
                onSnapped.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "baseCube")
        {            
            isSnapped = false;
        }
    }

    public void MakeThisKinematic()                         //Makes the object kinematic so that it can snap in place.
    {        

        if (rb.isKinematic && !isSnapped)                   //If already kinematic and not snapped, disable kinematic
        {
            rb.isKinematic = false;
        }
        else if(isSnapped)
        {
            rb.isKinematic = true;
        }
    }

}
