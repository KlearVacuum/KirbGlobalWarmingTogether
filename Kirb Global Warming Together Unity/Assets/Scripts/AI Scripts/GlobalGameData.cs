using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalGameData
{
    public static int cash;
    public static List<AIEntity> allAiEntities = new List<AIEntity>();
    public static List<GameObject> allTrash = new List<GameObject>();
    public static List<GameObject> allDepo = new List<GameObject>();

    // Call in game manager at awake to initialize all data
    public static void ResetGameData()
    {
        cash = 0;
        allAiEntities.Clear();
        allTrash.Clear();
        allDepo.Clear();
    }
    public static void AddTrash(GameObject trash)
    {
        if (allTrash.Contains(trash)) return;
        allTrash.Add(trash);
    }

    // Call when trash is destroyed
    public static void RemoveTrash(GameObject trash)
    {
        if (allTrash.Contains(trash)) allTrash.Remove(trash);
    }

    public static List<GameObject> NearbyTrash(Vector3 position, float radius)
    {
        List<GameObject> foundTrash = new List<GameObject>();
        foreach (GameObject trash in allTrash)
        {
            if (Vector3.Distance(position, trash.transform.position) < radius)
            {
                foundTrash.Add(trash);
            }
        }
        return foundTrash;
    }

    public static void AddDepo(GameObject depo)
    {
        if (allDepo.Contains(depo)) return;
        allDepo.Add(depo);
    }
    public static void RemoveDepo(GameObject depo)
    {
        if (allDepo.Contains(depo)) allDepo.Remove(depo);
    }

    public static List<GameObject> NearbyDepos(Vector3 position, float radius)
    {
        List<GameObject> foundDepos = new List<GameObject>();
        foreach (GameObject depo in allDepo)
        {
            if (Vector3.Distance(position, depo.transform.position) < radius)
            {
                foundDepos.Add(depo);
            }
        }
        return foundDepos;
    }

    public static void AddAiEntity(AIEntity ai)
    {
        if (allAiEntities.Contains(ai)) return;
        allAiEntities.Add(ai);
    }

    public static void RemoveAiEntity(AIEntity ai)
    {
        if (allAiEntities.Contains(ai)) allAiEntities.Remove(ai);
    }

    public static List<AIEntity> NearbyAiEntities(Vector3 position, float radius)
    {
        List<AIEntity> foundEnemies = new List<AIEntity>();
        foreach (AIEntity ai in allAiEntities)
        {
            if (Vector3.Distance(position, ai.transform.position) < radius)
            {
                foundEnemies.Add(ai);
            }
        }
        return foundEnemies;
    }
}