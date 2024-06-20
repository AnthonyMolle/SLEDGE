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
    public float verticalOffset = 5f;
    public float secondsBetweenStateUpdate = 0.5f;

    // Pathing trackers
    int currentPathIndex = -1;
    Vector3 currentPathPoint;
    bool pathing = false;

    // Logic Trackers
    Vector3 offset;
    Vector3 currentKnownTargetPos;
    float secondsSinceLastUpdate = 0;

    protected override void OnStart()
    {
        rootTransform = blackboard.getCurrentRunner().transform;
        rootRigidbody = blackboard.getCurrentRunner().GetComponent<Rigidbody>();
        rootCollider = blackboard.getCurrentRunner().GetComponent<BoxCollider>();

        currentTarget = blackboard.getObject(objectTarget);

        offset = new Vector3(0, verticalOffset, 0);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        checkForStateUpdate();

        if (child != null)
        {
            child.state = State.Running;
            child.Update();
        }

        if (isTargetInView())
        {
            if (pathing)
            {
                pathing = false;
                rootCollider.enabled = true;
            }
            return moveTowardsTarget();
        }
        else
        {
            if (pathing == false) setupPathing();
            return followPathToTarget();
        }
    }


    // Direct Movement --------------------


    private State moveTowardsTarget()
    {
        moveTowardsPoint(currentKnownTargetPos);

        if (reachedPoint(currentKnownTargetPos))
        {
            // Update knowledge and check again
            currentKnownTargetPos = currentTarget.transform.position + offset;
            secondsSinceLastUpdate = 0;

            // If reached this point, we have got to target!
            if (reachedPoint(currentKnownTargetPos))
            {
                rootRigidbody.velocity = new Vector3(0, 0, 0);
                return State.Success;
            }
        }

        return State.Running;
    }



    // Pathing --------------------



    private void setupPathing()
    {
        currentPathIndex = PlayerTracker.getPathIndex(rootTransform.position);
        currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
        rootCollider.enabled = false;
        pathing = true;
    }

    private State followPathToTarget()
    {
        moveTowardsPoint(currentPathPoint);

        if (reachedPoint(currentPathPoint))
        {
            currentPathIndex += 1;

            bool pathFinished = PlayerTracker.getSize() <= currentPathIndex;

            if (pathFinished)
            {
                currentPathIndex = PlayerTracker.getPathIndex(rootTransform.position);
            }

            currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
        }

        return State.Running;
    }


    // Helper methods --------------------


    private bool reachedPoint(Vector3 goal)
    {
        return Vector3.Distance(rootTransform.position, goal) < 1;
    }

    private void checkForStateUpdate()
    {
        secondsSinceLastUpdate += Time.deltaTime;
        if (secondsSinceLastUpdate >= secondsBetweenStateUpdate)
        {
            currentKnownTargetPos = currentTarget.transform.position + offset;
            secondsSinceLastUpdate = 0;
        }
    }

    private void moveTowardsPoint(Vector3 currentTarget)
    {
        Vector3 targetDirection = (currentTarget - rootTransform.position).normalized;

        rootRigidbody.velocity = targetDirection * speed;

        var targetRotation = Quaternion.LookRotation(currentTarget - rootTransform.position);
        rootTransform.rotation = Quaternion.Slerp(rootTransform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    private bool isTargetInView()
    {
        RaycastHit hit;

        // Raycasts all four corners of our object...
        // Ensures target is not only visible but reachable...
        // No blocks are blocking path!

        Vector3[] offsets = {Vector3.zero,Vector3.left,Vector3.right,Vector3.up,Vector3.down};

        foreach (Vector3 x in offsets)
        {
            Vector3 startPoint = rootTransform.position + x;
            Vector3 dirToTarget = currentTarget.transform.position - startPoint;

            if (Physics.Raycast(startPoint, dirToTarget, out hit, Mathf.Infinity, 1<<0 | 1 << 12))
            {
                if (hit.transform != currentTarget.transform)
                {
                    return false;
                }
            }
        }

        return true;
    }
}