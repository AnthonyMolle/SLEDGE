using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFromObject_Decorator : DecoratorNode
{
    public Blackboard.ObjectOptions objectTarget;
    public float distance;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        GameObject target;

        target = blackboard.getObject(objectTarget);

        if(Vector3.Distance(target.transform.position, blackboard.getCurrentRunner().transform.position) < distance)
        {
            child.state = State.Running;
            return child.Update();
        }

        return State.Failure;
    }
}
