using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Jonah Ryan

public class PlayerTracker : MonoBehaviour
{
    [Header("Path Parameters")]
    public float secBetweenPathUpdates;
    public int totalPathMemory = 255;

    [Header("Dependencies")]
    public Transform player;

    [Header("Debug")]
    public bool DebugLines = false;

    public float distanceBoundary = 10f;

    List<Vector3> playerPath = new List<Vector3>();
    

    static PlayerTracker instance;


    // Start is called before the first frame update
    void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        } else {
            instance = this;
        }

        StartCoroutine(trackPlayer());
    }

   
    public static Transform getPlayerTransform()
    {
        return instance.player;
    }
    
    public static Vector3 getPointFromIndex(int index)
    {
        return instance.playerPath[index];
    }

    public static int getPathIndex(Vector3 currentPosition)
    {
        int nearestIndex = 0;
        float distance = 0;

        for (int i = 0; i < instance.playerPath.Count; i++)
        {
            float d = Vector3.Distance(instance.playerPath[i], currentPosition);

            // Check if nodes are clos enough where we should prioritize order
            if(Math.Abs(d - distance) < instance.distanceBoundary)
            {
                if(nearestIndex < i)
                {
                    distance = d;
                    nearestIndex = i;
                }
            }

            if (d < distance || distance == 0)
            {
                distance = d;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    public static int getSize()
    {
        return instance.playerPath.Count;
    }

    IEnumerator trackPlayer()
    {
        Vector3 lastAdded = Vector3.zero;
        WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(secBetweenPathUpdates);
        while (true)
        {

            if (Vector3.Distance(lastAdded, player.position) > 1)
            {
                playerPath.Add(player.position);
                lastAdded = player.position;
            }

            if (playerPath.Count > totalPathMemory)
            {
                playerPath.RemoveAt(0);
            }

            yield return waitTime;
        }
    }

    void OnDrawGizmos()
    {

        if (DebugLines == false) return;

        Vector3 lastPoint = transform.position;

        foreach (Vector3 x in playerPath)
        {

            Gizmos.color = Color.green;
            Gizmos.DrawLine(lastPoint, x);

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(x, 0.2f);
            

            lastPoint = x;
        }

    }
}
