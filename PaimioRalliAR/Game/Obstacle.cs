using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    private CarMovement carMovement;


    private void Start()
    {
        carMovement = GameObjectManager.instance.allObjects[0].GetComponent<CarMovement>();
        
    }
    //Deactivates this object and calls obstacle collision method from carmovement
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
                        
            carMovement.ObstacleCollision(GameManager.instance.speedReduction);

            GameManager.instance.spawnParticles("parsys_SpeedSlowed");

            if (StaticVariables.vibrationOn)
            {
                Handheld.Vibrate();
            }

            gameObject.SetActive(false);
        }
    }

    //This method exist just to reactivate obstacles when a ground object is reused.
    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
