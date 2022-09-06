using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crown/Cash Mult")]
public class MultCrown : Crown
{
    public float trashCashMultiplier;

    public override void OnCrowned(AIEntity kirb)
    {
        kirb.mTrashCashMult = trashCashMultiplier;
    }
}
