using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.menuCanvas = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
