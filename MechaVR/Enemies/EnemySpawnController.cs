using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public int spawnAmount = 10;
    [HideInInspector] public List<Vector3> spawnPoints = new List<Vector3>();
    private List<EnemySpawnVolume> spawnVolumes = new List<EnemySpawnVolume>();

    private int remainingSpawnPoints;

    void Awake()
    {
        spawnVolumes.AddRange(GetComponentsInChildren<EnemySpawnVolume>());

        remainingSpawnPoints = spawnAmount;

        while (remainingSpawnPoints > 0)
        {
            foreach (EnemySpawnVolume spawnVolume in spawnVolumes)
            {
                if (remainingSpawnPoints > 0)
                    spawnVolume.AddSpawnPoint();

                remainingSpawnPoints--;
            }
        }
    }

    public void AddSpawnPoints(List<Vector3> points)
    {
        spawnPoints.AddRange(points);
    }

    public List<Vector3> GetSpawnPoints()
    {
        return spawnPoints;
    }
}
