using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode : Node
{
    [HideInInspector] public Node child; // Has one child, modifies childs properties.

    public override Node Clone()
    {
        ActionNode node = Instantiate(this);
        if (child != null)
        {
            node.child = child.Clone();
            node.state = State.Running;
        }
        return node;
    }
}
