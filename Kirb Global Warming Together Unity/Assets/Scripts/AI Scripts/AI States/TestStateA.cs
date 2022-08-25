using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStateA : AIState
{
    private float timer = 0;
    public TestStateA(string _name) : base (_name)
    {

    }

    public override bool EvaluateConditions(AIController aiController)
    {
        return true;
    }

    public override void StartState(AIController aiController)
    {
        timer = 1;
        canTransit = false;
    }

    public override void RunState(AIController aiController)
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Debug.Log(name + ": Ready to go");
            canTransit = true;
        }
    }

    public override void EndState(AIController aiController)
    {

    }
}
