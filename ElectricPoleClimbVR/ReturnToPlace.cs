using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPlace : MonoBehaviour
{
    private Quaternion startingRotation;
    
    public GameObject objectAnchor;                             //Original place of the object

    private Rigidbody rb;                                       //This objects rigidbody

    private Snapper snapper;                                    //This objects snapper script

    public bool objThrown = false;                              //Bool to determine if object was recently thrown

    [HideInInspector]
    public Vector3 defaultScale;

    // Start is called before the first frame update
    void Start()
    {
        startingRotation = transform.rotation;                  //Set current rotation as the starting rotation
        rb = GetComponent<Rigidbody>();
        snapper = GetComponent<Snapper>();
        defaultScale = transform.localScale;
    }

    
    public void ReturnObject()                                  //Returns the object to its original place
    {
        rb.isKinematic = true;                                  //Set object as kinematic
        rb.useGravity = false;                                  //Disable gravity
        transform.SetParent(objectAnchor.transform);            //Set object as a child of original place
        transform.localPosition = Vector3.zero;                 //Zero the local position of object
        transform.rotation = startingRotation;                  //Rotate back to starting rotation
        objThrown = false;
        transform.localScale = defaultScale;
    }
   

    public void OnThrow()                                       //What happens when object gets thrown
    {
        if (!snapper || !snapper.isSnapped && name != "brokenInsulator")                     //If not snapped or object has no snapper script
        {
            rb.isKinematic = false;                             //Object is no longer kinematic
            rb.useGravity = true;                               //Enable gravity
            objThrown = true;
            StartCoroutine(CallReturnObject());                 //Start returning object after a delay
        }
        else if (name == "brokenInsulator")
        {
            rb.isKinematic = false;                             //Object is no longer kinematic
            rb.useGravity = true;                               //Enable gravity
        }
    }

    private IEnumerator CallReturnObject()                      //Calls return object script after a 3 second delay
    {
        yield return new WaitForSeconds(3f);

        ReturnObject();
    }
}
