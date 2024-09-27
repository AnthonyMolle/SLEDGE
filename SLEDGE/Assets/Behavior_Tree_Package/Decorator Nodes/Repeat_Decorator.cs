using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeat_Decorator : DecoratorNode
{
    public int repeatNumber;
    int timesExecuted = 0;

    protected override void OnStart()
    {
        timesExecuted = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        
        if(child.state == Node.State.Running)
        {
            child.Update();
            return State.Running;
        }
        
       
        if(timesExecuted >= repeatNumber - 1 && repeatNumber != -1)
        {
            return State.Success;
        }

        timesExecuted++;
        child.state = State.Running;

        return State.Running;
    }
}
