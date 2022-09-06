using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crown/Panic Speed")]
public class PanicSpeedCrown : Crown
{
    public float panicMoveSpeed;

    public override void OnCrowned(AIEntity kirb)
    {
        if (kirb.panic) kirb.mCurrentMoveSpeed = panicMoveSpeed;
        kirb.mPanicMoveSpeed = panicMoveSpeed;
    }
}
