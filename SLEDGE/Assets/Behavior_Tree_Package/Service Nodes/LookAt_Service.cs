using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LookAt_Service : ServiceNode
{
    public Blackboard.ObjectOptions toLookAt;

    GameObject objectLookingAt;
    GameObject trackingPoint;


    protected override void OnStart()
    {
        objectLookingAt = blackboard.getObject(toLookAt);

        EnemyStatsController stats = blackboard.getCurrentRunner().GetComponent<EnemyStatsController>();
        if (stats == null)
        {
            Debug.LogError("Trying to run a LookAt Service but missing EnemyStatsController.");
        }

        trackingPoint = stats.lookAtTarget;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // Move tracking point to player/tracking target
        Transform currentRunnerTransform = blackboard.getCurrentRunner().transform;
        trackingPoint.transform.position = objectLookingAt.transform.position;

        Vector3 targetpos = new Vector3(trackingPoint.transform.position.x, 0, trackingPoint.transform.position.z);
        Vector3 pos = new Vector3(currentRunnerTransform.position.x, 0, currentRunnerTransform.position.z);
        float angle = Vector3.Angle(objectLookingAt.transform.position - currentRunnerTransform.position, currentRunnerTransform.forward);
        if (angle > 130)
        {
            currentRunnerTransform.rotation = Quaternion.LookRotation(targetpos - pos);
        }

        return State.Running;
    }
}