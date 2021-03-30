using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    //This Script enable or disable the differents rooms of the enviroment.
    public GameObject enviroment; // Enviroment is where the player is, so while the player is in the area of ENVIROMENT the room will be visible.



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {
            enviroment.gameObject.SetActive(true);
        }
        else
            enviroment.gameObject.SetActive(false);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enviroment.gameObject.SetActive(false);
        }
        else 
            enviroment.gameObject.SetActive(true);
        
    }


}
