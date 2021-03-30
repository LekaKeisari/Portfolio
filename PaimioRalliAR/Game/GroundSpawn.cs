using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject groundPrefab;

    public GroundsList groundsList;

    private Vector3 groundSpawnPosition;
    private GameObject lastGroundObject;
    private GameObject groundToActivate;
    public float groundHeight;
    private int switchCase = 0;


    public int groundCounter = 3;
    [HideInInspector]
    public int objectIndex = 5;         //Tracks which ground object was spawned last

    [Range(3, 20)]
    [SerializeField]
    private int level1Length;

    [Range(3, 20)]
    [SerializeField]
    private int level2Length;

    [HideInInspector]
    public int activeGrounds = 3;

    private int level3StartPoint;



    private void Start()
    {
        groundToActivate = groundsList.level1Grounds[Random.Range(1, groundsList.level1Grounds.Count - 1)];

        level3StartPoint = level1Length + level2Length;
    }

    //Activates a new ground object, calculates the right position, moves it there and selects then next object to activate when another ground object collides with the GroundSpawnTrigger

    public void addMoreGround()
    {
        TrackLastGroundObject();
        groundHeight = lastGroundObject.GetComponent<BoxCollider>().bounds.size.z;
        groundSpawnPosition = new Vector3(0, 0, lastGroundObject.transform.position.z + groundHeight);
        SpawnGround();
        PickRandomGround();
    }

    //Activates a new ground object, moves it to the right position and adds +1 to counter
    public void SpawnGround()
    {
        groundToActivate.SetActive(true);
        groundToActivate.transform.position = groundSpawnPosition;
        groundCounter++;

        //Changes the switch case variable depending on how many ground objects have been used
        if (groundCounter == level1Length)
        {
            groundToActivate = groundsList.levelSwitches[0];
        }
        if (groundCounter > level1Length && groundCounter < level3StartPoint)
        {
            switchCase = 1;
        }
        if (groundCounter == level3StartPoint)
        {
            groundToActivate = groundsList.levelSwitches[1];
        }
        if (groundCounter > level3StartPoint)
        {
            switchCase = 2;
        }
    }
    //Finds the last ground object on the list
    public void TrackLastGroundObject()
    {
        lastGroundObject = GameObjectManager.instance.allObjects[objectIndex];
    }

    //Picks a random ground object from a list depending on switch case variable
    private void PickRandomGround()
    {
        int randomTries = 0;

        while (groundToActivate.activeInHierarchy)
        {
            if (randomTries <= 10)
            {
                switch (switchCase)
                {
                    case 1:
                        int i = Random.Range(1, groundsList.level2Grounds.Count);
                        groundToActivate = groundsList.level2Grounds[i];
                        break;
                    case 2:
                        int j = Random.Range(1, groundsList.level3Grounds.Count);
                        groundToActivate = groundsList.level3Grounds[j];
                        break;

                    default:
                        int l = Random.Range(1, groundsList.level1Grounds.Count);
                        groundToActivate = groundsList.level1Grounds[l];
                        break;
                }
            }
            else
            {
                switch (switchCase)
                {
                    case 1:
                        for (int index = 0; index < groundsList.level2Grounds.Count; index++)
                        {
                            if (groundsList.level2Grounds[index].activeInHierarchy == false)
                            {
                                groundToActivate = groundsList.level2Grounds[index];
                                break;
                            }
                        }
                        break;
                    case 2:
                        for (int index = 0; index < groundsList.level3Grounds.Count; index++)
                        {
                            if (groundsList.level3Grounds[index].activeInHierarchy == false)
                            {
                                groundToActivate = groundsList.level3Grounds[index];
                                break;
                            }
                        }
                        break;

                    default:
                        for (int index = 0; index < groundsList.level1Grounds.Count; index++)
                        {
                            if (groundsList.level1Grounds[index].activeInHierarchy == false)
                            {
                                groundToActivate = groundsList.level1Grounds[index];
                                break;
                            }
                        }
                        break;
                }
            }

            randomTries = randomTries + 1;
        }

        activeGrounds = activeGrounds + 1;
    }
}
