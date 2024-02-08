using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using NaughtyAttributes;
using UnityEngine.UIElements;

public class AStarPathfinding : MonoBehaviour
{
    public Transform seeker, target;

    AStarGrid grid;

    void Awake()
    {
        grid = GetComponent<AStarGrid>();
    }

    [Button]
    public void StartPathFinding()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        AStarNode startNode = grid.NodeFromWorldPoint(startPos);
        AStarNode targetNode = grid.NodeFromWorldPoint(targetPos);

        AStarHeap<AStarNode> openSet = new AStarHeap<AStarNode>(grid.MaxSize);
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode) // Path Found
            {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + "ms");
                // Retrace our path found
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (AStarNode neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode> ();

        AStarNode currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(AStarNode a, AStarNode b)
    {
        return Mathf.RoundToInt(Vector3.Distance(a.worldPosition, b.worldPosition));
    }
}

