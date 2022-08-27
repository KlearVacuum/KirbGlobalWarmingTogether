using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Move To Trash State")]
public class MoveToTrashState : MoveToState
{
    public float searchRange;
    public MoveToTrashState(string _name) : base(_name)
    {

    }
    public override void Initialize(AIController aiController)
    {
        restartOnEvaluate = true;
    }
    public override bool EvaluateConditions(AIController aiController)
    {
        if (!aiController.ai.collectTrash) return false;
        if (aiController.ai.returnToDepo) return false;

        return FoundNearest(aiController);
    }

    private bool FoundNearest(AIController aiController)
    {
        GameObject nearestTrash = aiController.ai.GetNearestVisibleTrash(searchRange);
        if (nearestTrash == null) return false;
        aiController.ai.moveToTarget = nearestTrash.transform;
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
        }
        else
        {
            canTransit = true;
        }
    }
    public override void EndState(AIController aiController)
    {

    }
}
