using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "AI State/MoveTo State")]
public class MoveToState : AIState
{
    public MoveToState(string _name) : base(_name)
    {

    }
    public override bool EvaluateConditions(AIController aiController)
    {
        // Set condition to move towards object
        return true;
    }

    public override void StartState(AIController aiController)
    {
        canTransit = false;
        // find and set target in aiEntity
        // if target is null, canTransit = true
    }
    public override void RunState(AIController aiController)
    {
        // if target != null, move to target
    }
    public override void EndState(AIController aiController)
    {
        // set target to null
        // aiController.ai.SetAgentStop(false);
    }
}
