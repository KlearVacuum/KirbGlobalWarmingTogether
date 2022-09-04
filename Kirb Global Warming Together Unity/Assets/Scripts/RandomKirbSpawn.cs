using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomKirbSpawn : MonoBehaviour
{
    public List<GameObject> kirbs;
    public Vector2 posVariance;

    private void Start()
    {
        int rand = Random.Range(0, kirbs.Count);
        Instantiate(kirbs[rand], transform.position + new Vector3(Random.Range(-posVariance.x, posVariance.x), 
                                                                    Random.Range(-posVariance.y, posVariance.y), 0), Quaternion.identity);
    }
}
