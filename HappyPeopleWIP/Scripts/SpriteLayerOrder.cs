using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteLayerOrder : MonoBehaviour
{

    public SortingGroup sortGroup;
    //public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //player = player.GetComponent<GameObject>();
        sortGroup = sortGroup.GetComponent<SortingGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        sortGroup.sortingOrder = (int)(transform.position.z * -100);
    }
}
