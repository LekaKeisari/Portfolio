using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundFollowsPlayer : MonoBehaviour
{
    
    private GameObject player;
    private CarMovement carMovement;        
    private CameraShake cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObjectManager.instance.allObjects[0];
        carMovement = player.GetComponent<CarMovement>();
        cameraShake = GameObjectManager.instance.allObjects[6].GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, 0, player.transform.position.z);                //Move with player on z-axis
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody.tag == "Player" && carMovement.jumpIsHigh)                      //If jump was high enough trigger landing effects
        {
            GameManager.instance.spawnParticles("parsys_Landing");
            StartCoroutine(cameraShake.ShakeCamera(true, true, false, .5f, .5f));
            carMovement.jumpIsHigh = false;
            GameSoundManager.instance.PlayLandingSound();
        }
    }
}
