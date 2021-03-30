using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inside_Outside_Controller : MonoBehaviour
{
    //This Script enable or disable yard.
    public GameObject yard;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            yard.gameObject.SetActive(false);
        }

        else
            yard.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            yard.gameObject.SetActive(true);
        }

        else
            yard.gameObject.SetActive(false);
    }
}
