using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    #region Character Component References
    [Header("Character Component References")]
    Rigidbody rb;
    #endregion

    #region Camera
    [Header("Camera")]
    [SerializeField] GameObject cameraObject;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float camRotateAmountLR = 10f;
    [SerializeField] float camRotateAmountFB = 20f;
    [SerializeField] float camRotateSpeed = 17f;
    #endregion

    #region Input
    [Header("Input")]
    Vector3 movementInputVector;
    Vector2 mouseInputVector;
    float xRotation;
    float yRotation;
    #endregion

    #region Player Settings
    [Header("Player Settings")]
    [SerializeField] float mouseSensitivity = 1;
    #endregion

    #region Movement
    [Header("Movement")]
    [SerializeField] float accelerationRate = 10f;
    [SerializeField] float decelerationRate = 10f;
    [SerializeField] float maxSpeed = 100f;
    [SerializeField] int maxSlopeAngle = 45;
    #endregion

    #region Air Movement
    [Header("Air Movement")]
    [SerializeField] float airAccelerationRate = 10f;
    [SerializeField] float airDecelerationRate = 10f;
    [SerializeField] float airMaxSpeed = 100f;
    #endregion

    #region Jump
    [Header("Jump")]
    bool jumpPressed = false;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float coyoteTime = 0.25f;
    #endregion

    #region Raycast Checks
    [Header("Raycast Checks")]
    bool isGrounded = false;
    bool isOnSlope = false;
    [SerializeField] float playerHeight = 1f;
    [SerializeField] float groundCheckDist = 0.2f;
    //[SerializeField] float slopeCheckDist = 0.3f;
    [SerializeField] LayerMask groundLayers;
    #endregion

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpPressed = false;
        }

        //all upcoming code could be put somewhere way better or in a function but its cool ok lol
        float xCamRotate;
        float zCamRotate;

        if (horizontalInput > 0.4)
        {
            zCamRotate = -camRotateAmountLR;
        }
        else if (horizontalInput < -0.4)
        {
            zCamRotate = camRotateAmountLR;
        }
        else
        {
            zCamRotate = 0;
        }
        
        if (verticalInput > 0.4)
        {
            xCamRotate = camRotateAmountFB;
        }
        else if (verticalInput < -0.4)
        {
            xCamRotate = -camRotateAmountFB;
        }
        else
        {
            xCamRotate = 0;
        }

        //MAKE IT SO FORWARD AND LEFT RIGHT CAMROTATION SPEEDS ARE DIFFERENT
        cameraHolder.transform.localRotation = Quaternion.RotateTowards(cameraHolder.transform.localRotation, Quaternion.Euler(xCamRotate, 0, zCamRotate), camRotateSpeed * Time.deltaTime);
        
    }

    private void HandleMovement()
    {
        #region Raycast Checks
        //all this raycast slope stuff is getting kinda out of hand
        RaycastHit hit;
        Vector3 movementPlane;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, playerHeight/2 + groundCheckDist, groundLayers))
        {
            isGrounded = true;
            movementPlane = hit.normal;

            if (Vector3.Angle(hit.transform.up, hit.normal) > 1)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
            }
        }
        else
        {
            movementPlane = transform.up;
            isGrounded = false;
            isOnSlope = false;
        }

        #endregion

        #region Ground Movement
        
        Vector3 flatVelocity;

        if (isOnSlope)
        {
            movementInputVector = Vector3.ProjectOnPlane(movementInputVector, movementPlane);
        }


        flatVelocity = Vector3.ProjectOnPlane(rb.velocity, movementPlane);

        Debug.Log(flatVelocity.magnitude);

        if (movementInputVector.magnitude > 0.001)
        {
            if (flatVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(movementInputVector * accelerationRate);

                if (flatVelocity.magnitude > maxSpeed)
                {
                    rb.velocity = new Vector3((movementInputVector * maxSpeed).x, rb.velocity.y, (movementInputVector * maxSpeed).z); //Vector3.ProjectOnPlane(new Vector3((movementInputVector * maxSpeed).x, rb.velocity.y, (movementInputVector * maxSpeed).z), movementPlane);
                }
            }
            else
            {
                rb.velocity = new Vector3((movementInputVector * maxSpeed).x, rb.velocity.y, (movementInputVector * maxSpeed).z); //Vector3.ProjectOnPlane(new Vector3((movementInputVector * maxSpeed).x, rb.velocity.y, (movementInputVector * maxSpeed).z), movementPlane);
            }
        }
        else
        {
            if (flatVelocity.magnitude > 0.01)
            {
                rb.AddForce(-flatVelocity * decelerationRate);
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0); //Vector3.ProjectOnPlane(new Vector3(0, rb.velocity.y, 0), movementPlane);
            }
        }
        #endregion

        #region Jump
        if (jumpPressed && isGrounded)
        {
            Debug.Log("jump!");
        }
        #endregion
    }

    private void Jump()
    {

    }
}
