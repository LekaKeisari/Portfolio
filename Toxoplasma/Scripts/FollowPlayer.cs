using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;

    private GameManager gameManager;
    private CameraMovement mainCam;
    [HideInInspector]
    public float cameraRotation;
    [SerializeField]
    private float followSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        mainCam = gameManager.mainCamera;
        if (gameManager.cameraTargetRotation != null)
        {
            cameraRotation = gameManager.cameraTargetRotation.eulerAngles.y;
        }

        if (!player)
        {
            player = GameManager.instance.player;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameManager.cameraFollowDisabled)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, followSpeed);
            transform.rotation = Quaternion.Euler(0, cameraRotation, 0);
        }
    }
}
