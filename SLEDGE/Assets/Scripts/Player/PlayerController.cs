using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    [Header("Character Component References")]
    Rigidbody rb;

    [Header("Camera")]
    [SerializeField] GameObject cameraObject;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float camRotateAmount = 10f;

    [Header("Input")]
    Vector3 movementInputVector;
    Vector2 mouseInputVector;
    float xRotation;
    float yRotation;

    [Header("Player Settings")]
    [SerializeField] float mouseSensitivity = 1;

    [Header("Movement")]
    [SerializeField] float accelerationRate = 10f;
    [SerializeField] float decelerationRate = 10f;
    [SerializeField] float maxSpeed = 100f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        HandleLookRotation();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLookRotation()
    {
        //rotating player body left and right
        yRotation += mouseInputVector.x;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        //rotating camera up and down
        xRotation -= mouseInputVector.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraObject.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    private void HandleInput()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSensitivity;

        mouseInputVector = new Vector2(mouseX, mouseY);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        movementInputVector = ((horizontalInput * transform.right) + (verticalInput * transform.forward)).normalized;

        //all upcoming code could be put somewhere way better or in a function but its cool ok lol
        float xCamRotate;
        float zCamRotate;

        if (horizontalInput > 0.4)
        {
            zCamRotate = -camRotateAmount;
        }
        else if (horizontalInput < -0.4)
        {
            zCamRotate = camRotateAmount;
        }
        else
        {
            zCamRotate = 0;
        }
        
        if (verticalInput > 0.4)
        {
            xCamRotate = camRotateAmount;
        }
        else if (verticalInput < -0.4)
        {
            xCamRotate = -camRotateAmount;
        }
        else
        {
            xCamRotate = 0;
        }

        cameraHolder.transform.localRotation = Quaternion.RotateTowards(cameraHolder.transform.localRotation, Quaternion.Euler(xCamRotate, 0, zCamRotate), 17 * Time.deltaTime);
        
    }

    private void HandleMovement()
    {
        Debug.Log(movementInputVector * accelerationRate);
        rb.AddForce(movementInputVector * accelerationRate);
    }
}
