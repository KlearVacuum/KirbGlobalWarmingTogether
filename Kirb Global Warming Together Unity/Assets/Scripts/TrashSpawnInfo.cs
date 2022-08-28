using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trash Spawning/Trash Spawn Info")]
public class TrashSpawnInfo : ScriptableObject
{
    public List<GameObject> TrashToSpawn;
    public int NumLevels = 1;
    public int Quantity = 40;
}
