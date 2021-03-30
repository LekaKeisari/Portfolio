using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawnVolume : MonoBehaviour
{
    private List<Vector3> spawnPoints = new List<Vector3>();
    private EnemySpawnController spawnController;
    private BoxCollider spawnVolume;
    private int volumeSpawnAmount;

    void Start()
    {
        spawnController = GetComponentInParent<EnemySpawnController>();
        spawnVolume = GetComponent<BoxCollider>();

        for (int i = 0; i < volumeSpawnAmount; i++)
        {
            Vector3 spawnPoint = Vector3.zero;

            spawnPoint.x = Random.Range(-spawnVolume.size.x / 2f, spawnVolume.size.x / 2f);
            spawnPoint.y = 0f;
            spawnPoint.z = Random.Range(-spawnVolume.size.z / 2f, spawnVolume.size.z / 2f);

            spawnPoint = transform.TransformPoint(spawnPoint);

            spawnPoints.Add(spawnPoint);
            Debug.DrawRay(spawnPoint, Vector3.up * 2f, Color.red, 10f);
        }
        spawnController.AddSpawnPoints(spawnPoints);
    }

    public void AddSpawnPoint()
    {
        volumeSpawnAmount++;
    }

 
}
