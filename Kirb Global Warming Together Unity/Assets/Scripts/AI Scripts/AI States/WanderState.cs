using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Wander State")]
public class WanderState : AIState
{
    public WanderState(string _name) : base(_name)
    {

    }
    public override bool EvaluateConditions(AIController aiController)
    {
        // wandering around looking for stuff to do
        return aiController.ai.collectTrash;
    }

    public override void StartState(AIController aiController)
    {
        aiController.ai.moveToTarget = aiController.ai.forwardDir;
        aiController.ai.EnableVelVar(true);
        canTransit = true;
    }
    public override void RunState(AIController aiController)
    {
        aiController.ai.MoveTowardTarget();
    }
    public override void EndState(AIController aiController)
    {
        aiController.ai.EnableVelVar(false);
    }
}