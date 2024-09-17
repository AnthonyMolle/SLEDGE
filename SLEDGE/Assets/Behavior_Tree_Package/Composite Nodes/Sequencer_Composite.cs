using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Execute each of the children until one fails
// Node fails if a child fails
// Node succeeds if all children pass!
public class Sequencer_Composite : CompositeNode
{
    int current;

    protected override void OnStart()
    {
        current = 0;
        foreach (Node node in children)
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
                return State.Failure;
            case State.Success:
                current++;
                break;
        }

        return current == children.Count ? State.Success : State.Running;
    }
}
