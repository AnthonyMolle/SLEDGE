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
        child.state = State.Running;
        child.Update();
        return State.Success;
    }
}
