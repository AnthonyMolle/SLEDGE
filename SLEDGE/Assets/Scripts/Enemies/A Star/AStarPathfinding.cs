using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using NaughtyAttributes;
using UnityEngine.UIElements;
using System.Collections;
using System;

// 3D A* Implementation 
public class AStarPathfinding : MonoBehaviour
{

    AStar_Path_Request_Manager request_Manager;

    // Define our grid
    AStarGrid grid;

    void Awake()
    {
        request_Manager = GetComponent<AStar_Path_Request_Manager>();
        grid = GetComponent<AStarGrid>();
    }

    // Used by our Path Request manager to request a path to be processed
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }


    // Find path from startPos -> targetPos
    /*
     * Note pathfinding uses two heuristics:
     * - gCost is a nodes shortest distance from startPos
     * - hCost is a nodes estimated distance from our targetPos
     * 
     * - hCost is found by taking the Vector3 distance from our target to our current node.
     *   This does not account for geometry however it gauges which nodes to visit next well.
     * 
     */
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Time ms path finding takes
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // Track our path
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // Find nodes our parameters are inside
        AStarNode startNode = grid.NodeFromWorldPoint(startPos);
        AStarNode targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable) { 
            yield return null;
            request_Manager.FinishedProcessingPath(waypoints, pathSuccess);
        }
        // Store nodes to be visited by A*
        AStarHeap<AStarNode> openSet = new AStarHeap<AStarNode>(grid.MaxSize);

        // Store nodes visited by A*
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

        // StartNode is first node to visit
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Pop closest to target node using our 2 heuristics
            AStarNode currentNode = openSet.RemoveFirst();

            // Marked current as visited
            closedSet.Add(currentNode);

            // Check if path found
            // Visiting our target = path found
            if (currentNode == targetNode)
            {
                // Display ms pathfinding took
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + "ms");

                // Retrace our path found
                pathSuccess = true;
                break;
            }

            // For all nodes X: nodes 1 away of our current node...
            foreach (AStarNode neighbour in grid.GetNeighbours(currentNode))
            {
                // Skip X: if collisions occure in node boundary or already visited by A*
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                // Get X distance from start relative to currentNode
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                // If found distance from start -> currentNode is less than previous stored
                // Then update X to our new gCost (distance from start -> X) 
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Update heuristic
                    neighbour.gCost = newMovementCostToNeighbour;
                    
                    // Calculate estimated distance from goal: hCost
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    // Remember currentNode as parent to retrace steps in final path trace
                    neighbour.parent = currentNode;

                    // Add X as possible visitable nodes in future
                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        request_Manager.FinishedProcessingPath(waypoints,pathSuccess);
    }

    // Once FindPath is processed...
    // We can then trace a path from the parent data in each AStarNode
    Vector3[] RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode> ();

        // Start with our end node and work backwards
        AStarNode currentNode = endNode;

        // Until we find our startNode...
        while(currentNode != startNode)
        {
            // Add our current node to our path
            path.Add(currentNode);

            // Go backwards from the parent
            currentNode = currentNode.parent;
        }

        // Remove points that go the same direction
        Vector3[] waypoints = SimplifyPath(path);

        // Flip path for usability
        Array.Reverse(waypoints);


        return waypoints;
    }

    Vector3[] SimplifyPath(List<AStarNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector3 directionOld = Vector3.zero;

        for(int i = 1; i < path.Count; i++)
        {
            Vector3 directionNew = new Vector3(
                path[i - 1].gridPos_x - path[i].gridPos_x,
                path[i - 1].gridPos_y - path[i].gridPos_y,
                path[i - 1].gridPos_z - path[i].gridPos_z);

            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    // Heuristic calculation for hCost: estimated distance from goal
    int GetDistance(AStarNode a, AStarNode b)
    {
        return Mathf.RoundToInt(Vector3.Distance(a.worldPosition, b.worldPosition));
    }
}

