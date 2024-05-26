using UnityEngine;

public class DebugLog_Action : ActionNode
{
    public string message;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate{message}");
        blackboard.moveToPosition.x += 1;
        Debug.Log($"OnUpdate{blackboard.moveToPosition}");

        
        return State.Success;
    }
}
