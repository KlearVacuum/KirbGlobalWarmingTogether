using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirbSpawnScript : MonoBehaviour
{
    public KirbType kirbType;

    public void SetKirbType()
    {
        GameManager._instance.kirb = kirbType.kirb;
        GameManager._instance.kirbCost = kirbType.cost;
        // GameManager._instance.kirbName = kirbType.name;
    }
}
