using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepoScript : MonoBehaviour
{
    void Start()
    {
        GlobalGameData.AddDepo(gameObject);
    }

    public void RemoveDepo()
    {
        Debug.Log("depo is gone");
        GlobalGameData.RemoveDepo(gameObject);
        Destroy(this);
    }
}