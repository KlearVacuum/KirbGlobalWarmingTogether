using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// State manager of AI
public class AIController : MonoBehaviour
{
    private AIEntity mAI;
    public AIEntity ai { get { return mAI; } }
    public string currentStateName;

    public List<AIState> allAIStates = new List<AIState>();

    private AIState prevState;
    protected AIState currentState;
    protected AIState nextState;

    private string stateResults;

    private void Awake()
    {
        mAI = GetComponent<AIEntity>();
    }
    private void Start()
    {
        InitializeStates();
        prevState = currentState = nextState = allAIStates[0];
        currentState.StartState(this);
    }

    protected virtual void Update()
    {
        if (allAIStates.Count == 0)
        {
            Debug.LogError(gameObject.name + " AI Controller has no AI states!");
        }
        if (nextState == currentState)
        {
            // Debug.Log("Current State: " + currentState.name);
            currentStateName = currentState.name;
            currentState.RunState(this);
            if (currentState.canTransit)
            {
                EvaluateStates();
            }
        }
        else
        {
            currentState.EndState(this);
            nextState.StartState(this);
            currentState = nextState;
        }
        prevState = currentState;
    }

    public virtual void InitializeStates()
    {

    }

    public void EvaluateStates()
    {
        stateResults = "Evaluation: \n";
        for (int i = 0; i < allAIStates.Count; ++i)
        {
            stateResults += allAIStates[allAIStates.Count - i - 1].name + ":" + allAIStates[allAIStates.Count - i - 1].EvaluateConditions(this).ToString() + "\n";
            if (allAIStates[allAIStates.Count - i - 1].EvaluateConditions(this))
            {
                nextState = allAIStates[allAIStates.Count - i - 1];
                break;
            }
        }
        // Uncomment this code to take a peek at what the kirbs be thinking about - but be warned it'll be quite a MESS if there's more than 1 active
        // PrintStateEvaluation();
    }

    public void PrintStateEvaluation()
    {
        Debug.Log(stateResults);
    }

    public void ForceTransition()
    {
        currentState.canTransit = true;
    }
}