using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Random Panic State")]
public class RandomPanicState : MoveToState
{
    public Vector2 minMaxMoveRange;
    [Tooltip("min/max values for changing destination; shorter values look more panicky")]
    public Vector2 minMaxTime;
    public RandomPanicState(string _name) : base(_name)
    {

    }
    public override bool EvaluateConditions(AIController aiController)
    {
        return aiController.ai.panic;
    }

    public override void StartState(AIController aiController)
    {
        canTransit = false;
    }
    public override void RunState(AIController aiController)
    {
        aiController.ai.PanicRandomMoveToTargets(minMaxMoveRange, minMaxTime);
        if (!aiController.ai.panic) canTransit = true;
    }
    public override void EndState(AIController aiController)
    {
    }
}
