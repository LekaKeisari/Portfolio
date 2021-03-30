using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    private CarMovement carMovement;

    private void Start()
    {
        carMovement = GameObjectManager.instance.allObjects[0].GetComponent<CarMovement>();     //Gets object from a singletons list
        
    }

    //Deactivates this object and adds value to speed on collision with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.boostMode = true;                                              //Lets GameManager know that boostmode is on
            carMovement.boostEndTime = Time.time + GameManager.instance.speedBoostDuration;     //Sets ending time for boost mode: current time + boosts duration
            GameManager.instance.spawnParticles("parsys_SpeedBoost");                           //Spawns particle system from objectpool
            gameObject.SetActive(false);
            GameSoundManager.instance.PlayItemSound(GameSoundManager.Item.Boost);
        }
    }


    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
