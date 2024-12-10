using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    // Let Designer choose which axis gets spun
    [SerializeField] bool spinX = false;
    [SerializeField] bool spinY = false;
    [SerializeField] bool spinZ = false;

    // Let Designer choose speed of specific spinning axis
    [SerializeField] float xSpeed = 0;
    [SerializeField] float ySpeed = 0;
    [SerializeField] float zSpeed = 0;

    void Update() // Update is called once per frame
    {
        // Spin Desired axis (or axises) by specified speed
        if(spinX)
        {
            transform.Rotate(xSpeed * Time.deltaTime, 0, 0, Space.Self);
        }
        if(spinY)
        {
            transform.Rotate(0, ySpeed * Time.deltaTime, 0, Space.Self);
        }
        if(spinZ)
        {
            transform.Rotate(0, 0, zSpeed * Time.deltaTime, Space.Self);
        }
    }
}
