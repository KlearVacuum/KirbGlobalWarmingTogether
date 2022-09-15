using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crown/Panic Time")]
public class PanicTimeCrown : Crown
{
    public float panicDuration;

    public override void OnCrowned(AIEntity kirb)
    {
        kirb.mPanicDuration = panicDuration;
    }
}