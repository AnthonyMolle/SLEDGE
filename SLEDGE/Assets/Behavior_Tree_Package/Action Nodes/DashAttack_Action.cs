using System;
using System.Reflection;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class DashAttack_Action : ActionNode
{
    Transform rootTransform;
    Vector3 attackDirection;
    GameObject target;

    public Blackboard.ObjectOptions objectTarget;
    public float speed;

    protected override void OnStart()
    {
        rootTransform = blackboard.getCurrentRunner().transform;
        Rigidbody rootRigidbody = blackboard.getCurrentRunner().GetComponent<Rigidbody>();

        target = blackboard.getObject(objectTarget);

        attackDirection = (target.transform.position - rootTransform.position).normalized;

        rootRigidbody.velocity = attackDirection * speed;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        RaycastHit hit;

        if (child != null)
        {
            child.state = State.Running;
            child.Update();
        }

        bool HitCollidable = Physics.Raycast(rootTransform.position, attackDirection, out hit, 2, 1 << 0 | 1 << 12);
        if (HitCollidable)
        {
            if (hit.transform == target.transform)
            {
                // Do damage to target, or trigger callback function
            }

            return State.Success;
        }
      
        return State.Running;
    }
}
