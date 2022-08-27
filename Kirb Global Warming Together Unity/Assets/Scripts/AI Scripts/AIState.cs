using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "AI State/State Name")]
public class AIState : ScriptableObject
{
    public new string name;

    // canTransit tells the ai controller to re-evaluate for a new state, ie this state can now be transited out of
    [HideInInspector]
    public bool canTransit;

    public bool restartOnEvaluate = false;

    public AIState(string _name)
    {
        name = _name;
    }

    public virtual void Initialize(AIController aiController)
    {

    }

    public virtual bool EvaluateConditions(AIController aiController) { return true; }

    // First frame when this state is active
    public virtual void StartState(AIController aiController) { }
    // Updates every frame while this state is active
    public virtual void RunState(AIController aiController) { }
    // Last frame when this state is active, before first frame of next state
    public virtual void EndState(AIController aiController) { }
}