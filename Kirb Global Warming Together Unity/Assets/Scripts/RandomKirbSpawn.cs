using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SPECIFICALLY USED FOR MAIN MENU ONLY 
public class RandomKirbSpawn : MonoBehaviour
{
    public int qty;
    public List<GameObject> kirbs;
    public Vector2 posVariance;

    private void Start()
    {
        for (int i = 0; i < qty; ++i)
        {
            int rand = Random.Range(0, kirbs.Count);
            GameObject kirb = Instantiate(kirbs[rand], transform.position + new Vector3(Random.Range(-posVariance.x, posVariance.x),
                                                                                        Random.Range(-posVariance.y, posVariance.y), 0), 
                                                                                        Quaternion.identity);
            AIEntity ai = kirb.GetComponent<AIEntity>();
            ai.crownByTimeWeightage = 0;
            ai.crownByTrashCostWeightage = 0;

            if (Random.Range(0,100) > 80)
            {
                ai.mCrownThreshold = -1;
            }
        }
    }
}
