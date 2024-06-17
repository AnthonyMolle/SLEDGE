using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class UpdateBlackboard_Action : Blackboard_ActionNode
{
    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        ourValue = getBlackboardValue();
        if (ourValue == null) return State.Failure;

        currentField_blackboard.SetValue(blackboard, ourValue);

        if (child != null)
        {
            child.state = State.Running;
            child.Update();
        }
        return State.Success;
    }
}