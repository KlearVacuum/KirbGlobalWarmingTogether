using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Move To Depo State")]
public class MoveToDepoState : MoveToState
{
    public float searchRange;
    public MoveToDepoState(string _name) : base(_name)
    {
    }

    public override void Initialize(AIController aiController)
    {
        restartOnEvaluate = true;
    }

    public override bool EvaluateConditions(AIController aiController)
    {
        // Set condition to move towards object
        if (!aiController.ai.returnToDepo) return false;

        return FoundNearest(aiController);
    }

    private bool FoundNearest(AIController aiController)
    {
        GameObject nearestDepo = aiController.ai.GetNearestVisibleDepo(searchRange);
        if (nearestDepo == null) return false;
        aiController.ai.moveToTarget = nearestDepo.transform;
        return true;
    }

    public override void StartState(AIController aiController)
    {
        canTransit = false;
        aiController.ai.ResetTravelTime();
    }

    public override void RunState(AIController aiController)
    {
        if (aiController.ai.TravelTimeExceeded())
        {
            aiController.ai.ResetTravelTime();
            canTransit = true;
            FoundNearest(aiController);
        }

        if (aiController.ai.moveToTarget != null)
        {
            aiController.ai.MoveTowardTarget();

            // arrived at depo
            if (aiController.ai.depoOverlap)
            {
                aiController.ai.Deposit();
                canTransit = true;
            }
        }
        else
        {
            Debug.Log("here");
            canTransit = true;
        }
    }
    public override void EndState(AIController aiController)
    {

    }
}
