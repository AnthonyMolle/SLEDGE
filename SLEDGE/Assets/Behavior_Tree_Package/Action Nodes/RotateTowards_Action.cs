using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotateTowards_Action : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        blackboard.getCurrentRunner().transform.LookAt(blackboard.getObjectA().transform);
        return State.Running;
    }
}