using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlatform : MonoBehaviour
{

    [SerializeField] private bool OneSpeed; // Checks to see if we want a unified speed
    [SerializeField] private bool rotateX; // Checks to see if we want to rotate X
    [SerializeField] private bool rotateY; // Checks to see if we want to rotate Y
    [SerializeField] private bool rotateZ; // Checks to see if we want to rotate Z
    [SerializeField] private float speed; // Holds the unified speed
    [SerializeField] private float speedX; // Holds the speed of X rotation
    [SerializeField] private float speedY; // Holds the speed of Y rotation
    [SerializeField] private float speedZ; // Holds the speed of Z rotation

    [SerializeField] private bool updateRotation; // Checks to see if we want to update what axis gets rotated

    private Vector3 rotateAxises; // Holds a list of the axises that get rotated

    // Start is called before the first frame update

    void Start()
    {
        updateRotation = false; // dont cheeck update rotation before we start. should always be false on start
        UpdateRotation(); // Update the rotation with what the player has inputted
    }

    // Update is called once per frame
    void Update()
    {
        if (OneSpeed) // if we want one speed, rotate all rotateable axis by the same speed
        {
            transform.Rotate(rotateAxises.x * speed * Time.deltaTime, rotateAxises.y * speed * Time.deltaTime, rotateAxises.z * speed * Time.deltaTime);
            
        }
        else // else, rotate all rotateable axises by their seperate speeds
        {
            transform.Rotate(rotateAxises.x * speedX * Time.deltaTime, rotateAxises.y * speedY * Time.deltaTime, rotateAxises.z * speedZ * Time.deltaTime);
        }

        if (updateRotation) // if were trying to update the rotation, update it then reset variable
        {
            UpdateRotation();
            updateRotation = false;
        }
    }

    void UpdateRotation()
    {
        // clear the vector, then use the booleans to see which axises can be rotated
        rotateAxises = new Vector3(0, 0, 0);
        if (rotateX) { rotateAxises.x = 1; }
        if (rotateY) { rotateAxises.y = 1; }
        if (rotateZ) { rotateAxises.z = 1; }
    }
}
