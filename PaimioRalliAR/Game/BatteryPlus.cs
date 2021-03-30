using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPlus : MonoBehaviour
{       
    //Deactivates this object and adds value to batterylife on collision with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (GameManager.instance.batteryLife + (GameManager.instance.batteryBoost * GameManager.instance.maxBatteryLife) > GameManager.instance.maxBatteryLife)
            {
                GameManager.instance.batteryLife = GameManager.instance.maxBatteryLife;
            }
            else
            {
                GameManager.instance.batteryLife += GameManager.instance.batteryBoost * GameManager.instance.maxBatteryLife;   //Adds a precentage of maximum batterylife to current batterylife
            }
            GameManager.instance.spawnParticles("parsys_BatteryBoost");                                                    //Spawns particle system from objectpool
            GameSoundManager.instance.PlayItemSound(GameSoundManager.Item.Battery);
            gameObject.SetActive(false);
        }
    }


    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
