using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// limited time: remove all weaknesses, tough to drown
[CreateAssetMenu(menuName = "Crown/Time Limited Super")]
public class TimeLimitedSuperCrown : Crown
{
    public float drownTime;
    public float timeToDie;

    public override void OnCrowned(AIEntity kirb)
    {
        kirb.timeToDrown = drownTime;

        kirb.trashTypeWeakness.Clear();
    }

    public override void CrownedUpdate(AIEntity kirb)
    {
        if (!kirb.dead)
        {
            kirb.superTimer += Time.deltaTime;
            if (kirb.superTimer > timeToDie)
            {
                kirb.forceStateTransition = true;
                kirb.deathType = eDeathType.NASTYFOOD;
                kirb.dead = true;
            }
        }
    }
}
