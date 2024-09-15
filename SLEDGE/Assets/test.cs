using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class test : MonoBehaviour
{
    Vector3 Camera_BottomLeft;
    Vector3 Camera_BottomRight;
    Vector3 Camera_TopLeft;
    Vector3 Camera_TopRight;

// Start is called before the first frame update
void Start()
    {
        Camera camera = Camera.main;

        float h = Screen.height;
        float w = Screen.width;

        float cam_x = camera.transform.position.x;
        float cam_y = camera.transform.position.y;

        Camera_BottomLeft = new Vector3(cam_x - w, cam_y - h,0);
        Camera_BottomRight = new Vector3(cam_x + w, cam_y - h,0);
        Camera_TopLeft = new Vector3(cam_x - w, cam_y + h,0);
        Camera_TopRight = new Vector3(cam_x + w, cam_y + h,0);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Camera_BottomLeft, 4);
        Gizmos.DrawSphere(Camera_BottomRight, 4);
        Gizmos.DrawSphere(Camera_TopLeft, 4);
        Gizmos.DrawSphere(Camera_TopRight, 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
