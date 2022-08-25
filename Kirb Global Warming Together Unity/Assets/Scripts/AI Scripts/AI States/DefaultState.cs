using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Default State")]
public class DefaultState : AIState
{
    public DefaultState(string _name) : base(_name)
    {

    }
    public override bool EvaluateConditions(AIController aiController)
    {
        return true;
    }

    public override void StartState(AIController aiController)
    {
        canTransit = true;
    }
    public override void RunState(AIController aiController)
    {

    }
    public override void EndState(AIController aiController)
    {

    }
}
