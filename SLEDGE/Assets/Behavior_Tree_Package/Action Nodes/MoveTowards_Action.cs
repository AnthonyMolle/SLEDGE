using System;
using UnityEngine;

public class MoveTowards_Action : ActionNode
{
    // Dependencies
    Transform rootTransform;
    Rigidbody rootRigidbody;
    BoxCollider rootCollider;
    GameObject currentTarget;

    // Public Parameters
    public Blackboard.ObjectOptions objectTarget;
    public float speed;

    // Internal State tracking
    int currentPathIndex = -1;
    Vector3 currentPathPoint;
    bool pathing = false;

    bool targetInView = false;

    protected override void OnStart()
    {
        rootTransform = blackboard.getCurrentRunner().transform;
        rootRigidbody = blackboard.getCurrentRunner().GetComponent<Rigidbody>();
        rootCollider = blackboard.getCurrentRunner().GetComponent<BoxCollider>();

        currentTarget = blackboard.getObject(objectTarget);

        targetInView = isTargetInView();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Debug.Log("Target in view is: " + targetInView);

        if (isTargetInView())
        {
            MoveTowardsTarget(currentTarget.transform.position);
            rootCollider.enabled = true;

            if (pathing) pathing = false;

            targetInView = isTargetInView();

        }
        else
        {
            if (pathing == false)
            {
                currentPathIndex = PlayerTracker.getPathIndex(rootTransform.position);
                currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
                rootCollider.enabled = false;
                pathing = true;
            }

            bool reachedPoint = Vector3.Distance(rootTransform.position, currentPathPoint) < 1;

            if (reachedPoint)
            {
                targetInView = isTargetInView();

                currentPathIndex += 1;

                bool pathFinished = PlayerTracker.getSize() <= currentPathIndex;

                if (pathFinished)
                {
                    currentPathIndex = PlayerTracker.getPathIndex(rootTransform.position);
                }

                currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
            }

            MoveTowardsTarget(currentPathPoint);
        }


        if (child != null)
        {
            child.state = State.Running;
            child.Update();
        }

        return State.Running;
    }

    private void MoveTowardsTarget(Vector3 currentTarget)
    {
        Vector3 targetDirection = (currentTarget - rootTransform.position).normalized;

        rootRigidbody.velocity = targetDirection * speed;
        blackboard.getCurrentRunner().transform.LookAt(currentTarget);
    }

    private bool isTargetInView()
    {
        RaycastHit hit;

        Vector3[] offsets = {Vector3.zero,Vector3.left,Vector3.right,Vector3.up,Vector3.down};

        int fails = 0;

        foreach (Vector3 x in offsets)
        {
            Vector3 startPoint = rootTransform.position + x;
            Vector3 dirToTarget = currentTarget.transform.position - startPoint;
            Debug.DrawRay(startPoint, dirToTarget);

            if (Physics.Raycast(startPoint, dirToTarget, out hit, Mathf.Infinity, 1<<0 | 1 << 12))
            {
                if (hit.transform != currentTarget.transform)
                {
                    Debug.Log("Actually hit: " + hit.transform.gameObject.name);
                    return false;
                }
            }
        }

        return true;
    }
}