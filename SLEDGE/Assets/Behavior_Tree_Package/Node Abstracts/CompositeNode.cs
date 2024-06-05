using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    public List<Node> children = new List<Node>(); // Chooses one of the children to run!

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.state = State.Running;
        node.started = false;
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}
