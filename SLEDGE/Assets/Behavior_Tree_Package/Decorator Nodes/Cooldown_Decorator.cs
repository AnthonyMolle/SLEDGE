using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown_Decorator : DecoratorNode
{
    public float secondsOfCooldown;

    float sinceCooldown;
    bool onCooldown = false;

    protected override void OnStart()
    {
        if (onCooldown && Time.time - sinceCooldown >= secondsOfCooldown)
        {
            onCooldown = false;
        }

        if (onCooldown == false)
        {
            child.state = State.Running;
        }
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (onCooldown)
        {
            return State.Failure;
        }

        if(child.state == Node.State.Running)
        {
            child.Update();
            return State.Running;
        }

        // Child finished

        onCooldown = true;
        sinceCooldown = Time.time;

        return child.state;
    }
}
