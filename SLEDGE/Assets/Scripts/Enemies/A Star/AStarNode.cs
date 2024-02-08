using UnityEngine;

// Basic Node class for AStar
// Stores data for each grid cell
public class AStarNode : IHeapItem<AStarNode>
{
    public bool walkable; // Mark if node has geometry in area or not
    public Vector3 worldPosition; // World coordinate of center of AStar 

    // Index in grid along each axis
    public int gridPos_x;
    public int gridPos_y;
    public int gridPos_z;

    public int gCost; // Known shortest distance from start
    public int hCost; // Estimated distance from goal
    public AStarNode parent; // Stores node gCost comes from
    int heapIndex;

    // Constructer for node
    public AStarNode(bool walkable, Vector3 worldPos, int gridPos_x, int gridPos_y, int gridPos_z)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridPos_x = gridPos_x;
        this.gridPos_y = gridPos_y;
        this.gridPos_z = gridPos_z;
    }

    // fCost is total of both Heuristics, minHeap sorted by fCost
    public int fCost {
        get{ return gCost + hCost; }
    }

    // HeapIndex monitors where we are in the minHeap, sorted by fCost
    public int HeapIndex
    {
        get{ return heapIndex; }
        set{ heapIndex = value; }
    }

    // Compare two nodes fCost, then just hCost if need be
    public int CompareTo(AStarNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        // Inverts to use object based compare instead of int compare
        return -compare;
    }
}
