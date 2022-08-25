using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Move To Depo State")]
public class MoveToDepoState : MoveToState
{
    public float searchRange;
    public float depositRange;
    public MoveToDepoState(string _name) : base(_name)
    {

    }
    public override bool EvaluateConditions(AIController aiController)
    {
        // Set condition to move towards object
        if (!aiController.ai.returnToDepo) return false;
        GameObject nearestDepo = aiController.ai.GetNearestDepo(searchRange);
        if (nearestDepo == null) return false;
        aiController.ai.moveToTarget = nearestDepo.transform;
        return true;

    }

    public override void StartState(AIController aiController)
    {
        canTransit = false;
    }
    public override void RunState(AIController aiController)
    {
        if (aiController.ai.moveToTarget != null)
        {
            aiController.ai.MoveTowardTarget();

            // arrived at depo
            if (Vector2.Distance(aiController.ai.moveToTarget.position, aiController.ai.transform.position) < depositRange)
            {
                aiController.ai.Deposit();
                canTransit = true;
            }
        }
        else
        {
            canTransit = true;
        }
    }
    public override void EndState(AIController aiController)
    {
        aiController.ai.moveToTarget = null;
    }
}
