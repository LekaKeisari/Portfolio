using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{

    public Transform goal;

    public NavMeshAgent agent;
    private List<Vector3> spawnPoints;

    void Start()
    {
        spawnPoints = GameManager.instance.spawnController.GetSpawnPoints();
        agent = GetComponent<NavMeshAgent>();
        goal = GameManager.instance.player.transform;
        //agent.Warp(new Vector3(Random.Range(-20f, 10f), 0f, Random.Range(28f, 32f)));
        agent.Warp(spawnPoints[Random.Range(0, spawnPoints.Count - 1)]);

        //if (gameObject.transform.position.y > 3f)
        //{
        //    gameObject.GetComponent<NavMeshAgent>().Warp(new Vector3(Random.Range(-20f, 60f), 0f, Random.Range(28f, 32f)));
        //}

    }

    private void Update()
    {
        if (Vector3.Distance(agent.transform.position, goal.position) > 4f )
        {
            agent.destination = goal.position;
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
