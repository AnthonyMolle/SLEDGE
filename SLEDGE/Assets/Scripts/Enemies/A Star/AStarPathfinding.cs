using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{

    AStarGrid grid;

    void Awake()
    {
        grid = GetComponent<AStarGrid>();
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        AStarNode startNode = grid.NodeFromWorldPoint(startPos);
        AStarNode targetNode = grid.NodeFromWorldPoint(targetPos);

        List<AStarNode> openSet = new List<AStarNode>();
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost
                    || openSet[i].fCost == currentNode.fCost
                    && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode) // Path Found
                {
                    return;
                }

                foreach (AStarNode neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode,neighbour)

                    if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        
                    }

                }
            }
        }
    }

    int GetDistance(AStarNode a, AStarNode b)
    {
        // Create huristic of my own
        return Mathf.RoundToInt(Vector3.Distance(a.worldPosition, b.worldPosition));
    }
}

