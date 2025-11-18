using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireAttachment : MonoBehaviour
{
    public List<GameObject> partsToAttach = new List<GameObject>();
    public List<Transform> attachPoints = new List<Transform>();
    public bool attachOnStart = true;

    void Start()
    {
        if (attachOnStart)
        {
            AttachParts();
        }
    }

    public void AttachParts()
    {
        for (int i = 0; i < partsToAttach.Count && i < attachPoints.Count; i++)
        {
            if (partsToAttach[i] != null && attachPoints[i] != null)
            {
                partsToAttach[i].transform.position = attachPoints[i].position;
                partsToAttach[i].transform.rotation = attachPoints[i].rotation;
            }
        }
    }
}
