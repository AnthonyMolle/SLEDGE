using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarNode : IHeapItem<AStarNode>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridPos_x;
    public int gridPos_y;
    public int gridPos_z;

    public int gCost;
    public int hCost;
    public AStarNode parent;
    int heapIndex;

    public AStarNode(bool walkable, Vector3 worldPos, int gridPos_x, int gridPos_y, int gridPos_z)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridPos_x = gridPos_x;
        this.gridPos_y = gridPos_y;
        this.gridPos_z = gridPos_z;
    }

    public int fCost {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(AStarNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
