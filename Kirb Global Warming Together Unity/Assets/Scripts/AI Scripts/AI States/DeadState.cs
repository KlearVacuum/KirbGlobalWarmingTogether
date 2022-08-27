using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Dead State")]
public class DeadState : MoveToState
{
    public eDeathType deathType;
    public DeadState(string _name) : base(_name) { }

    public override bool EvaluateConditions(AIController aiController)
    {
        return aiController.ai.dead && aiController.ai.deathType == deathType;
    }

    public override void StartState(AIController aiController)
    {
        aiController.ai.moveToTarget = null;
        canTransit = false;
        // can start death animation thingy here, then in animator can call kirb's death function
        aiController.ai.DampenVelocity(0.5f);
        aiController.ai.Die();
    }

    public override void RunState(AIController aiController)
    {

    }
    public override void EndState(AIController aiController)
    {

    }
}

public enum eDeathType
{
    DROWN,
    NASTYFOOD
}