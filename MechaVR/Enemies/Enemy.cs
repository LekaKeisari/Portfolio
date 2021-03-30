using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public float enemyHealth = 100;
    public float enemyDamage = 1f;

    private bool withinRange = false;

    private List<Vector3> spawnPoints;
    private void OnEnable()
    {
        spawnPoints = GameManager.instance.spawnController.GetSpawnPoints();
        StartCoroutine(CheckForHits());
    }

    private void Start()
    {
        
    }
    //private void Update()
    //{
    //    RaycastHit hit;
    //    // Does the ray intersect any objects excluding the player layer
    //    if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), out hit, 4f))
    //    {
    //        //Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
    //        if (hit.collider.tag == "Player")
    //        {
    //            //Debug.Log("Did Hit");
    //            withinRange = true;
    //            StartCoroutine(DamagePlayer(enemyDamage));
    //        }
    //    }

    //    else
    //    {
    //        withinRange = false;
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GotHit();
        }
    }


    public void Spawn()
    {                
        //gameObject.GetComponent<NavMeshAgent>().Warp(new Vector3(Random.Range(-20f, 10f), 0f, Random.Range(28f, 32f)));  
       
        gameObject.GetComponent<NavMeshAgent>().Warp(spawnPoints[Random.Range(0, spawnPoints.Count - 1)]);                
    }

    //private void Update()
    //{
    //    if (gameObject.transform.position.y > 3f)
    //    {
    //        gameObject.GetComponent<NavMeshAgent>().Warp(new Vector3(Random.Range(-20f, 60f), 0f, Random.Range(28f, 32f)));
    //    }
    //}

    public void GotHit()
    {               
        GameManager.instance.Pool.ReleaseObject(gameObject);
        GameManager.instance.RemoveMonster(this);
        GameManager.instance.playerScore++;
        GameManager.instance.CheckNumberOfEnemies();
        //withinRange = false;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (other.tag == "projectile")
    //    //{
    //    //    GotHit();
    //    //}

    //    if (other.tag == "Player")
    //    {
    //        withinRange = true;
    //        StartCoroutine(DamagePlayer(enemyDamage));

    //        //GotHit();

    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        withinRange = false;
    //    }
    //}

    //haha peepee poopoo
    public void DealDamage(float damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            GotHit();
        }
    }

    public IEnumerator DamagePlayer(float damage)
    {
        while (withinRange)
        {
            GameManager.instance.playerHealth -= damage;

            if (GameManager.instance.playerHealth <= 0)
            {
                //GameManager.instance.gameOver = true;
                GameManager.instance.GameOver();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator CheckForHits()
    {
        while (gameObject.activeInHierarchy)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), out hit, 4f))
            {
                //Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                if (hit.collider.tag == "Player")
                {
                    //Debug.Log("Did Hit");
                    withinRange = true;
                    StartCoroutine(DamagePlayer(enemyDamage));
                }
            }

            else
            {
                withinRange = false;
            }

            yield return new WaitForSeconds(1f);
        }
        
    }
}
