using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour
{    
    public GroundSpawn groundSpawn;   

    private int timesActivated = 0;                              //Tells how many times object has been made active

    private GameObject player;

    public Component[] obstacles;

    public Component[] movableObjects;

    public Component[] batteryPluses;

    public Component[] boosts;



    void Update()
    {        
        //Deactivate ground object if distance to player is high enough
        if (player.transform.position.z - transform.position.z >= this.GetComponent<BoxCollider>().bounds.size.z * 1.5)
        {
            groundSpawn.activeGrounds = groundSpawn.activeGrounds - 1;
            this.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {        
        obstacles = GetComponentsInChildren<Obstacle>(true);      //Add all child obstacles to a list  
        movableObjects = GetComponentsInChildren<MovableObjectPositionReset>(true);
        batteryPluses = GetComponentsInChildren<BatteryPlus>(true);
        boosts = GetComponentsInChildren<Boost>(true);
    }

    private void OnEnable()
    {
        timesActivated++;                                        

        //If object has been activated for 2 or more times, get index number from the list
        if (timesActivated >= 2)
        {
            groundSpawn.objectIndex = GameObjectManager.instance.allObjects.IndexOf(this.gameObject);
        }
                
        //Activates all child obstacles
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.Activate();            
        }

        // Move all movableobjects bact to their original positons
        foreach (MovableObjectPositionReset movableObject in movableObjects)
        {
            movableObject.Reposition();
        }

        foreach (BatteryPlus batteryPlus in batteryPluses)
        {
            batteryPlus.Activate();
        }

        foreach (Boost boost in boosts)
        {
            boost.Activate();
        }
    }

    private void Start()
    {              
        groundSpawn = GameObjectManager.instance.allObjects[1].GetComponent<GroundSpawn>();
        player = GameObjectManager.instance.allObjects[0];
        
        //Adds the activated object to allObjects list. Doesn't add three pieces that have been placed on default.
        if (GameManager.instance.setupComplete)
        {            
            GameObjectManager.instance.allObjects.Add(gameObject);
            groundSpawn.objectIndex = GameObjectManager.instance.allObjects.IndexOf(this.gameObject);            
        }
    }
       
    //Check if the ground object has collided with the GroundSpawn object. Tells that initial setup is complete and deactivates this ground object.
    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.setupComplete = true;
  
        if (other.tag == "Player" && groundSpawn.activeGrounds <= 5)
        {
            groundSpawn.addMoreGround();
        }
    }    
}
