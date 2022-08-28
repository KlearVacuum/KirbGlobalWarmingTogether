using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawnerScript : MonoBehaviour
{
    public List<GameObject> trashToSpawn;
    public int quantity;
    public bool spawnAtStart;
    
    void Start()
    {
        if (spawnAtStart)
        {
            SpawnTrash();
        }
    }

    public void SpawnTrash()
    {
        if (trashToSpawn != null && trashToSpawn.Count > 0)
        {
            // randomly scatter trash within spawn area
            for (int i = 0; i < quantity; ++i)
            {
                int rand = Random.Range(0, 100);
                GameObject trash = trashToSpawn[rand % trashToSpawn.Count];
                float randX = Random.Range(0f, 100f) / 100f * transform.localScale.x - (transform.localScale.x / 2);
                float randY = Random.Range(0f, 100f) / 100f * transform.localScale.y - (transform.localScale.y / 2);

                randX += transform.position.x;
                randY += transform.position.y;

                Instantiate(trash, new Vector2(randX, randY), Quaternion.identity);
            }
        }
    }
}
