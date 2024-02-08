using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Script reference: https://www.youtube.com/watch?v=nhiFx28e7JY

public class AStarGrid : MonoBehaviour
{
    public bool onlyDisplayPathGizmos = true;
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector3 gridWorldSize; // Center point of grid
    public float nodeRadius; // How much space each node covers
    AStarNode[,,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY, gridSizeZ;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY * gridSizeZ;
        }
    }

    void CreateGrid()
    {
        grid = new AStarNode[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomLeft = transform.position
            - Vector3.right * gridWorldSize.x / 2
            - Vector3.up * gridWorldSize.y / 2
            - Vector3.forward * gridWorldSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                for(int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft
                        + Vector3.right * (x * nodeDiameter + nodeRadius)
                        + Vector3.up * (y * nodeDiameter + nodeRadius)
                        + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                    grid[x, y, z] = new AStarNode(walkable, worldPoint, x,y,z);
                }
            }
        }
    }

    public AStarNode NodeFromWorldPoint(Vector3 worldPos)
    {
        worldPos = worldPos - transform.position;

        float precentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float precentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float precentZ = (worldPos.z + gridWorldSize.z / 2) / gridWorldSize.z;
        precentX = Mathf.Clamp01(precentX);
        precentY = Mathf.Clamp01(precentY);
        precentZ = Mathf.Clamp01(precentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * precentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * precentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * precentZ);

        return grid[x, y, z];

    }

    public List<AStarNode> GetNeighbours(AStarNode node)
    {
        List<AStarNode> neighbours = new List<AStarNode>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    int checkPos_x = node.gridPos_x + x;
                    int checkPos_y = node.gridPos_y + y;
                    int checkPos_z = node.gridPos_z + z;

                    if(checkPos_x >= 0 && checkPos_x < gridSizeX
                        && checkPos_y >= 0 && checkPos_y < gridSizeY
                        && checkPos_z >= 0 && checkPos_z < gridSizeZ)
                    {
                        neighbours.Add(grid[checkPos_x,checkPos_y,checkPos_z]);
                    }
                }
            }
        }

        return neighbours;
    }

    public List<AStarNode> path;
    private void OnDrawGizmos()
    {

        if (onlyDisplayPathGizmos)
        {
            if(path != null)
            {
                foreach (AStarNode n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
            return;
        }

        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        if(grid != null)
        {
            AStarNode playerNode = NodeFromWorldPoint(player.position);

            foreach(AStarNode n in grid)
            {
                Gizmos.color = Color.red;

                if(playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }

                if(path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                    }
                }
                if (!n.walkable) Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

}
