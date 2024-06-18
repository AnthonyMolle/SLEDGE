using UnityEngine;

public class MoveTowards_Action : ActionNode
{
    // Dependencies
    Transform rootTransform;
    Rigidbody rootRigidbody;
    GameObject currentTarget;

    // Public Parameters
    public Blackboard.ObjectOptions objectTarget;
    public float speed;

    // Internal State tracking
    int currentPathIndex = -1;
    Vector3 currentPathPoint;
    bool pathing = false;

    protected override void OnStart()
    {
        rootTransform = blackboard.getCurrentRunner().transform;
        rootRigidbody = blackboard.getCurrentRunner().GetComponent<Rigidbody>();

        currentTarget = blackboard.getObject(objectTarget);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (isTargetInView())
        {
            MoveTowardsTarget(currentTarget.transform.position);

            if(pathing) pathing = false;
        }
        else
        {
            if (pathing == false)
            {
                currentPathIndex = PlayerTracker.getPathIndex(rootTransform.position);
                currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
                pathing = true;
            }

            bool reachedPoint = Vector3.Distance(rootTransform.position, currentPathPoint) < 1;

            if (reachedPoint)
            {
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
    }

    private bool isTargetInView()
    {
        Vector3 currentPosToTarget = currentTarget.transform.position - rootTransform.position;
        RaycastHit hit;

        // Everything blocks this infinite raycast
        if (Physics.Raycast(rootTransform.position, currentPosToTarget, out hit, Mathf.Infinity, ~0))
        {
            if (hit.transform == currentTarget.transform)
            {
                return true;
            }
        }
        return false;
    }
}