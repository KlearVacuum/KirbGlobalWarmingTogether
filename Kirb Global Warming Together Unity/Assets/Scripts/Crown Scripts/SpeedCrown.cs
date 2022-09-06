using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crown/Move Speed")]
public class SpeedCrown : Crown
{
    public float moveSpeed;

    public override void OnCrowned(AIEntity kirb)
    {
        if (!kirb.panic) kirb.mCurrentMoveSpeed = moveSpeed;
        kirb.mMoveSpeed = moveSpeed;
    }
}
