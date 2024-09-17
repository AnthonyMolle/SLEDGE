using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Execute each children until one succeedes
// Will fail if all children fail
public class Selector_Composite : CompositeNode
{
    int current;

    protected override void OnStart()
    {
        current = 0;
        foreach(Node node in children)
        {
            node.state = Node.State.Running;
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        var child = children[current];

        switch (child.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                current++;
                break;
            case State.Success:
                return State.Success;
        }

        return current == children.Count ? State.Failure : State.Running;
    }
}
