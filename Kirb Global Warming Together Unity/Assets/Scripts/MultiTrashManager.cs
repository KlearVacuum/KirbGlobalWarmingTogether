using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTrashManager : MonoBehaviour
{
    public List<TrashSpawnerScript> trashSpawners;

    public List<GameObject> trashToSpawn;
    public int quantity;
    public bool allSameTrashSetup;

    public void SetTrashToSpawn(TrashSpawnInfo spawnInfo) 
    {
        trashToSpawn = spawnInfo.TrashToSpawn;
        SetupAllTrashSpawners();
    }

    private void Awake()
    {
        SetupAllTrashSpawners();
    }

    private void SetupAllTrashSpawners()
    {
        // Setup all trash Spawners
        if (allSameTrashSetup)
        {
            foreach (TrashSpawnerScript spawner in trashSpawners)
            {
                spawner.trashToSpawn = trashToSpawn;
                spawner.quantity = quantity;
            }
        }
    }

    public void SpawnSomeTrash(int trashSpawnNum)
    {
        List<TrashSpawnerScript> remainingSpawners = trashSpawners;
        for (int i = 0; i < trashSpawnNum; ++i)
        {
            if (remainingSpawners.Count == 0) break;
            int rand = Random.Range(1, 1000) % remainingSpawners.Count;
            remainingSpawners[rand].SpawnTrash();
            remainingSpawners.Remove(remainingSpawners[rand]);
        }
    }
}
