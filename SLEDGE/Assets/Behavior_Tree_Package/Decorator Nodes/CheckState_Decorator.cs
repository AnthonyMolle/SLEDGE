using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckState_Decorator : DecoratorNode
{
    public Blackboard.EnemyStates stateToCheckFor;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // If not on state, fail
        if(blackboard.currentState != stateToCheckFor)
        {
            return State.Failure;
        }

        // Otherwise pass through the node execution

        return child.Update();
    }
}
