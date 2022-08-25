using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    public int trashCash;
    void Start()
    {
        GlobalGameData.AddTrash(gameObject);
    }

    public void RemoveTrash()
    {
        // Debug.Log("trash is gone");
        GlobalGameData.RemoveTrash(gameObject);
        Destroy(gameObject);
    }
}