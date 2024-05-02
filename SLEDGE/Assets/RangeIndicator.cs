using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private bool inRange = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPos(Vector3 point)
    {
        gameObject.transform.position = point;
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
    }
}
