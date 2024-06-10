using UnityEngine;

public class RotateTowards_Service : ServiceNode
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