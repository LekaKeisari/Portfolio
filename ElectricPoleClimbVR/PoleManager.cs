using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleManager : MonoBehaviour
{
    public static PoleManager instance = null;

    public Transform pole;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
