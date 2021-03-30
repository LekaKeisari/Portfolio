using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //[SerializeField]
    public GameObject cameraTarget;
    private GameManager gameManager;
       
    public Vector3 offset;

    public Quaternion defaultRotation;
    

    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.mainCamera = this;
        defaultRotation = transform.rotation;
        if (!gameManager.cameraFollowDisabled)
        {
            FollowTarget();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!gameManager.cameraFollowDisabled)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        //transform.position = cameraTarget.transform.position + offset;
        transform.rotation = cameraTarget.transform.rotation * Quaternion.Euler(40, cameraTarget.transform.rotation.y, cameraTarget.transform.rotation.z);
    }
}
