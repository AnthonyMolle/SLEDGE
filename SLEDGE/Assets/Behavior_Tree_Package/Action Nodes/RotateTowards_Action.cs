using CW.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Progress;

public class RotateTowards_Action : ActionNode
{
    public Blackboard.ObjectOptions rotateTarget;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        GameObject target;

        target = blackboard.getObject(rotateTarget);
        blackboard.getCurrentRunner().transform.LookAt(target.transform);
        return State.Running;
    }
}