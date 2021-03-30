using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingObject : MonoBehaviour
{
    public bool onGrass = false;

    private CarMovement carMovement;
    private GameObject player;

    private void Start()
    {
        player = GameObjectManager.instance.allObjects[0];
        carMovement = player.GetComponent<CarMovement>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !onGrass)
        {
            GameManager.instance.maxSpeed = GameManager.instance.maxSpeed * 0.5f;                   //Lowers maxspeed to half while the player is on grass

            onGrass = true;
            carMovement.ObstacleCollision(GameManager.instance.grassEffect);                        //Call obstacle collision method with grass effect parameter

            if (Mathf.Abs(transform.position.x) - Mathf.Abs(player.transform.position.x) < .1)      //If car is completely on grass play particle systems for both tires
            {
                carMovement.grassParticleSystemLeft.Play();
                carMovement.grassParticleSystemRight.Play();
            }

            if (Mathf.Abs(transform.position.x) - Mathf.Abs(player.transform.position.x) >= .1)     //If car is only halfway on grass play one tire depending on side
            {
                if (player.transform.position.x < 0)
                {
                    carMovement.grassParticleSystemLeft.Play();
                }
                if (player.transform.position.x > 0)
                {
                    carMovement.grassParticleSystemRight.Play();
                }
            }
        }

        //if car is partly on gravel set proper particle systems active
        if (Mathf.Abs(transform.position.x) - Mathf.Abs(player.transform.position.x) >= .1)         
        {
            if (player.transform.position.x < 0)
            {                
                carMovement.tireGrassParSysLeft.SetActive(true);
                carMovement.tireGravelParSysLeft.SetActive(false);
            }
            if (player.transform.position.x > 0)
            {                
                carMovement.tireGrassParSysRight.SetActive(true);
                carMovement.tireGravelParSysRight.SetActive(false);
            }                
        }

        if (Mathf.Abs(transform.position.x) - Mathf.Abs(player.transform.position.x) < .1)
        {            
            carMovement.tireGrassParSysLeft.SetActive(true);
            carMovement.tireGrassParSysRight.SetActive(true);
            carMovement.tireGravelParSysLeft.SetActive(false);
            carMovement.tireGravelParSysRight.SetActive(false);
        }     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !onGrass)
        {
            GameSoundManager.instance.PlayGrassSounds(true);

            if (StaticVariables.vibrationOn)
            {
                Handheld.Vibrate();
                
            }
        }
    }

    //When player leaves grass set maxspeed back to normal and handle particle systems accordingly
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && onGrass)
        {
            GameManager.instance.maxSpeed = GameManager.instance.maxSpeed * 2;

            carMovement.grassParticleSystemLeft.Stop();
            carMovement.grassParticleSystemRight.Stop();

            carMovement.tireGrassParSysLeft.SetActive(false);
            carMovement.tireGrassParSysRight.SetActive(false);
            carMovement.tireGravelParSysLeft.SetActive(true);
            carMovement.tireGravelParSysRight.SetActive(true);
            
            GameSoundManager.instance.PlayGrassSounds(false);

            onGrass = false;
        }
    }
}
