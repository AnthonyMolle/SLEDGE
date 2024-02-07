using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarNode
{
    public bool walkable;
    public Vector3 worldPosition;

    public AStarNode(bool walkable, Vector3 worldPos)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
    }
}
