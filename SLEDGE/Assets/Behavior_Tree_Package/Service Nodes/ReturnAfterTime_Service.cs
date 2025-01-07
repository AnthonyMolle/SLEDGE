using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ReturnAfterTime_Service : ServiceNode
{
    public bool returnSuccess = true;
    public float time = 1.0f;
    float timer = 0.0f;

    protected override void OnStart()
    {
        timer = 0.0f;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= time)
        {
            return returnSuccess ? State.Success : State.Failure;
        }
        return State.Running;
    }
}