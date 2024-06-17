using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait_Action : ActionNode
{
    public float duration = 1;
    float startTime;

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if(Time.time - startTime > duration)
        {
            return State.Success;
        }
        if (child != null)
        {
            child.state = State.Running;
            child.Update();
        }
        return State.Running;
    }
}
