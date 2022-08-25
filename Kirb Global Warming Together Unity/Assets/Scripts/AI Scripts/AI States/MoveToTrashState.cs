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
    public override bool EvaluateConditions(AIController aiController)
    {
        if (!aiController.ai.collectTrash) return false;
        if (aiController.ai.returnToDepo) return false;

        GameObject nearestTrash = aiController.ai.GetNearestTrash(searchRange);
        if (nearestTrash == null) return false;
        aiController.ai.moveToTarget = nearestTrash.transform;
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
