using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Action Delay Wait State")]
public class ActionDelayWaitState : MoveToState
{
    public ActionDelayWaitState(string _name) : base(_name)
    {

    }
    public override void Initialize(AIController aiController)
    {

    }
    public override bool EvaluateConditions(AIController aiController)
    {
        return !aiController.ai.ActionReady();
    }

    public override void StartState(AIController aiController)
    {
        canTransit = false;
    }
    public override void RunState(AIController aiController)
    {
        if (aiController.ai.ActionReady()) canTransit = true;
    }
    public override void EndState(AIController aiController)
    {

    }
}
