using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AStar_Path_Request_Manager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    AStarPathfinding pathfinder;
    bool isProcessingPath;

    static AStar_Path_Request_Manager instance;
    private void Awake()
    { 
        instance = this;
        pathfinder = GetComponent<AStarPathfinding>();  
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> call)
        {
            pathStart = start;
            pathEnd = end;
            callback = call;
        }
    }
    
    void tryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinder.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }
    
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        tryProcessNext();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);  
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.tryProcessNext();
    }
}
