using System.Collections.Generic;
using UnityEngine;

// Script reference: https://www.youtube.com/watch?v=nhiFx28e7JY

// Creates a 3D grid of AStarNodes that will be given properties required for A* navigation
// Like if a collision box is in the given square

public class AStarGrid : MonoBehaviour
{
    public bool displayGridGizmos = true; // Debug setting to draw our grid
    public Transform player;
    public LayerMask unwalkableMask; // Matches this layer for when we are checking collision occurrences in nodes
    public Vector3 gridWorldSize; // Center point of grid
    public float nodeRadius; // How much space each node covers
    AStarNode[,,] grid; // Our Grid

    float nodeDiameter;
    int gridSizeX, gridSizeY, gridSizeZ; // Number of nodes in each dimension

    private void Awake()
    {
        // Each node will take up nodeDiameter of space
        nodeDiameter = nodeRadius * 2;

        // Get # of nodes in each dimension
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        CreateGrid();
    }

    // Our graphs total # of nodes
    public int MaxSize
    {
        get{ return gridSizeX * gridSizeY * gridSizeZ; }
    }

    void CreateGrid()
    {
        // Create an empty 3D grid
        grid = new AStarNode[gridSizeX, gridSizeY, gridSizeZ];

        // Find bottom left position of our square
        Vector3 worldBottomLeft = transform.position
            - Vector3.right * gridWorldSize.x / 2
            - Vector3.up * gridWorldSize.y / 2
            - Vector3.forward * gridWorldSize.z / 2;

        // For each node in our grid...
        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                for(int z = 0; z < gridSizeZ; z++)
                {
                    // Find its position in the world space
                    Vector3 worldPoint = worldBottomLeft
                        + Vector3.right * (x * nodeDiameter + nodeRadius)
                        + Vector3.up * (y * nodeDiameter + nodeRadius)
                        + Vector3.forward * (z * nodeDiameter + nodeRadius);

                    // Walkable = true: when no collisions occure in the area of the node
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                    // Update grid with our node properties
                    grid[x, y, z] = new AStarNode(walkable, worldPoint, x,y,z);
                }
            }
        }
    }

    // Get which node a position falls in
    // If outside boundary: clamps to furthest possible point
    public AStarNode NodeFromWorldPoint(Vector3 worldPos)
    {
        // Adjust to center around grid
        worldPos = worldPos - transform.position;

        // Find relative offset precentage worldPos is to our grid
        float precentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float precentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float precentZ = (worldPos.z + gridWorldSize.z / 2) / gridWorldSize.z;

        // If worldPos outside of grid, bound to grid dimensions
        precentX = Mathf.Clamp01(precentX);
        precentY = Mathf.Clamp01(precentY);
        precentZ = Mathf.Clamp01(precentZ);

        // Find node using relative precentage
        int x = Mathf.RoundToInt((gridSizeX - 1) * precentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * precentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * precentZ);

        // Return matching node
        return grid[x, y, z];

    }

    // Return all grid neighbours of given node
    public List<AStarNode> GetNeighbours(AStarNode node)
    {
        List<AStarNode> neighbours = new List<AStarNode>();

        // For each node 1 unit away from node...
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    // Don't add our node as a neighbour
                    if (x == 0 && y == 0 && z == 0) continue;

                    // Get possible nodes grid index
                    int checkPos_x = node.gridPos_x + x;
                    int checkPos_y = node.gridPos_y + y;
                    int checkPos_z = node.gridPos_z + z;
                    
                    // Check if node grid index is in grid boundary...
                    if(checkPos_x >= 0 && checkPos_x < gridSizeX
                        && checkPos_y >= 0 && checkPos_y < gridSizeY
                        && checkPos_z >= 0 && checkPos_z < gridSizeZ)
                    {
                        // If so add to neighbours!
                        neighbours.Add(grid[checkPos_x,checkPos_y,checkPos_z]);
                    }
                }
            }
        }

        return neighbours;
    }

    // Draw out our grid
    private void OnDrawGizmos()
    {
        // Draws out our overall cube dimension
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        if(grid != null && displayGridGizmos == true)
        {
            // Find player pos in relation to our grid
            AStarNode playerNode = NodeFromWorldPoint(player.position);

            // Go through all nodes in grid...
            foreach(AStarNode n in grid)
            {
                Gizmos.color = Color.red;

                // Fill player node with cyan
                if(playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }

                // Draw collide boxes as red nodes
                if (!n.walkable) Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
