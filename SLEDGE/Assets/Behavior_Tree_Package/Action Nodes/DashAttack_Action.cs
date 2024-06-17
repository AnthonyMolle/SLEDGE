using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DashAttack_Action : ActionNode
{
    Transform currentTransform;
    Rigidbody currentRigidbody;

    GameObject target;

    public Blackboard.ObjectOptions objectTarget;
    public float distance;

    protected override void OnStart()
    {
        currentTransform = blackboard.getCurrentRunner().transform;
        currentRigidbody = blackboard.getCurrentRunner().GetComponent<Rigidbody>();

        target = blackboard.getObject(objectTarget);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // Do stuff

        if (child != null)
        {
            child.state = State.Running;
            child.Update();
        }
        return State.Success;
    }
}
