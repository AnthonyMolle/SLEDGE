using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using Random=UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    //EXPERIMENTAL SHIT
    [SerializeField] GameObject speedlines;
    [SerializeField] ParticleSystem speed;

    #region Character Component References
    [Header("Character Component References")]
    [SerializeField] Camera gameCamera;
    Rigidbody rb; // parent rigidbody
    CapsuleCollider characterCollider;
    [SerializeField] Animator anim; // parent animator
    #endregion

    #region Camera
    [Header("Camera")]
    [SerializeField] GameObject cameraObject;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float camRotateAmountLR = 10f;
    [SerializeField] float camRotateSpeed = 17f;
    #endregion

    #region Input
    [Header("Input")]
    Vector3 movementInputVector;
    Vector2 mouseInputVector;
    float xRotation;
    float yRotation;

    bool ctrlPressed = false;
    bool ctrlReleased = true;

    bool mousePressed = false;
    bool mouseReleased = true;

    bool secondaryPressed = false;
    bool secondaryReleased = false;
    #endregion

    #region Health and Spawning
    [Header("Health and Spawning")]
    [SerializeField] Checkpoint firstCheckpoint;
    int currentSpawnIndex = 0;
    Checkpoint currentCheckpoint;
    
    [SerializeField] int maxHealth = 3; // Maximum health the player can have
    private int currentHealth = 3; // Current health the player is at
    bool alive = true; // Tracks if the player is alive
    #endregion

    #region Player Settings
    [Header("Player Settings")]
    [SerializeField] public float mouseSensitivity = 1;
    #endregion

    #region Movement
    [Header("Movement")]
    [SerializeField] float accelerationRate = 10f;
    [SerializeField] float decelerationRate = 10f;
    [SerializeField] float maxSpeed = 100f;
    [SerializeField] int maxSlopeAngle = 45;
    bool isChangingDirection = false;
    #endregion

    #region Air Movement
    [Header("Air Movement")]
    [SerializeField] float airAccelerationRate = 10f;
    [SerializeField] float airBrakingAccelerationRate = 10f;
    [SerializeField] float airMaxSpeed = 100f;
    [SerializeField] float airMaxHammerSpeed = 100f;

    [SerializeField] float LaunchBrakingDelay = 0.5f;
    float LaunchBrakingTimer = 0.0f;

    //[SerializeField] float maxLaunchedSpeed = 100f;
    [SerializeField] float maxFallingSpeed = 100f;
    [Tooltip("natural rate our player will fall after the apex of their falling height.")]
    [SerializeField] float naturalAdditionalFallingSpeed = 4f;
    
    #endregion

    #region Jump
    
    bool jumpPressed = false;
    bool jumpHoldChecking = false;
    bool mustReleaseJump = false;

    [Header("Jump")]
    [SerializeField] float srcJumpPoint = 0.0f;
    [SerializeField] float jumpHoldCheckWindow = 0.25f;
    [SerializeField] float jumpForce = 2f;
    [SerializeField] float coyoteTime = 0.25f;
    [SerializeField] bool hasCoyoteTime = true;// THIS IS THE DEFAULT VALUE OF COYOTETIME
    [SerializeField] bool decreasingCoyoteTime = false;
    [SerializeField] bool hasJumped = false;
    #endregion

    #region Slide
    [Header("Slide")]
    [SerializeField] float slideHitboxHeight;
    [SerializeField] float slideCameraOffset;
    
    [SerializeField] float minSlideSpeed;
    [SerializeField] float flatSlideDrag;
    [SerializeField] float maxSlideDrag;
    [SerializeField] float maxSlideAcceleration;
    bool isSliding = false;

    #endregion

    #region Raycast Checks
    [Header("Raycast Checks")]
    bool isGrounded = false;
    bool isOnSlope = false;
    [SerializeField] float playerHeight = 1f;
    [SerializeField] float groundCheckDist = 0.2f;
    //[SerializeField] float slopeCheckDist = 0.3f;
    [SerializeField] LayerMask groundLayers;

    [SerializeField] float distanceCheckBuffer = 2.0f;

    [SerializeField] string hudDistance;

    float currentDistance = 0; // current distance between the player and a surface.
    bool isInRange = false; // represents if hammer is in range to hit an object
    #endregion

    #region Hammer
    [Header("Hammer")]
    [Tooltip("used for the bounce force of the first bounce")]
    [SerializeField] float initialBounceForce = 15f;

    [Tooltip("used for the Y bounce force of the first bounce")]
    [SerializeField] float initialBounceForceY = 15f;
    [SerializeField] float bounceForce = 10f;
    [Tooltip("How High we go when bouncing. set higher for faster acceleration, use with BounceGravity ")]
    [SerializeField] float bounceForceY = 10f;
    [Tooltip("Limits how high the player can bounce regardless of acceleration, increase when increasing bounce force and vice versa")]
    [SerializeField] float bounceGravity = 5f;
    [SerializeField] float bouncyUpForce = 10f;
    [SerializeField] float bouncyForce = 25f;
    [SerializeField] float hitLength = 5f;
    [SerializeField] float hitRadius = 1f;
    [Tooltip("How many additional bounces do we get when we hit air?")]
    [SerializeField] int maxAdditionalBounces = 0;
    int additionalBounces;

    [Tooltip("Collection of raycast points used to determine impact location of swing")]
    [SerializeField] GameObject[] hammerArcPoints;
    [Tooltip("Root of hammer swing arc, used to move the entire arc")]
    [SerializeField] GameObject hammerArcRoot;

    [SerializeField] GameObject impactPoint;

    [SerializeField] float chargeTime = 1f;
    [SerializeField] float hitTime = 1f;
    [SerializeField] float recoveryTime = 1f;

    [SerializeField] float swipeTime = 1f;
    [SerializeField] float swipeRecoveryTime = 0.5f;

    [SerializeField] float launchedRotationSpeed = 0.02f;

    [SerializeField] LayerMask bouncableLayers;

    [SerializeField] LayerMask swipeLayers;

    RaycastHit distanceCheck; // Checks to see the distance between the player and an object they are lookign at
    
    bool hammerBounced = false; // Tracks if hammer hit a surface
    bool isLaunched = false;

    bool hittingHammer = false; // Tracks if we are currently swinging our hammer
    bool chargingHammer = false;
    bool recovering = false;

    bool swipingHammer = false; // Tracks if the player used the secondary hammer action (swipe/parry)
    bool swipeRecovering = false;
    bool swipeComboReady = false;

    bool hammerCharged = false;
    bool hammerHit = false; // tracks if we just swung hammer
    bool hammerSwipe = false;

    Vector3 hitDirection;
    float swingForce;
    [SerializeField] float swipeForceBase = 20f;
    [SerializeField] float smashForceBase = 30f;

    float hammerTimer = 0;
    float hangTime = 0;
    float walkTime = 0;

    enum Combo 
    {
        notSwiping,
        Swipe1,
        Swipe2,
        Swipe3
    }
    Combo currentCombo = Combo.notSwiping;


    [SerializeField] float parriedProjectileSpeed = 1f;
    [SerializeField] float parriedProjectileLifetime = 10f;

    AudioManager audioManager;
    [SerializeField] GameObject HammerSound;

    private void Awake()
    {

    }

    #endregion

    #region UI
    [Header("UI")]
    public GameObject canvas;

    public TextMeshProUGUI displayDistance;
    public TextMeshProUGUI speedometer; // UI that displays how fast we are going
    public TextMeshProUGUI tempPowerupUI; // UI element that displays current equipped powerup
    public TextMeshProUGUI healthDisplay;

    [SerializeField] GameObject pause;

    [SerializeField] GameObject settings;

    [SerializeField] GameObject deathScreen;

    #endregion

    #region Power Ups
    public enum Powerup { None, Airburst, Explosive } // List of powerups in game (including none)

    Powerup currentPowerup; // Hold the currently equipped powerup

    [Header("Power Ups")]
    [Tooltip("How much we add to bounce force when the explosive powerup is enabled.")]
    public float explosiveForce;

    #endregion

    #region SpeedLines
    [SerializeField] float speedlineThreshold = 10f;
    [SerializeField] float fovThreshold = 20f;
    [SerializeField] float speedlineMax = 100f;
    float targetAlpha;
    [SerializeField] Camera[] cameras;
    [SerializeField] float initialFOV = 75;
    [SerializeField] float maxAdditionalFOV = 25;
    float targetFOV;
    [SerializeField] float initialParticleZ = 1;
    [SerializeField] float maxParticleZ = 4;
    float targetZ;
    [SerializeField] float initialParticleSpeed = 4;
    [SerializeField] float maxParticleSpeed = 20;
    float targetSpeed;
    [SerializeField] float initialParticleEmission = 25;
    [SerializeField] float maxParticleEmission = 200;
    float targetEmission;
    #endregion
   
    #region Parrying
    [SerializeField] float maxTargetAngle = 30f;
    [SerializeField] float minTargetDistance = 5f;
    [SerializeField] float maxTargetDistance = 50f;
    #endregion

    #region Events

    public UnityEvent onHammerBounce;
    [Tooltip("Invoked if the hammer hit was from an additional bounce.")]
    public UnityEvent onExtraHammerHit;

    #endregion
    [SerializeField] LayerMask enemyLayers;

    void Start() // Runs at the start of the Scene
    {
        // Set Player Audio Manager
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        // Lock the cursor to center screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Set RigidBody of Player
        rb = GetComponent<Rigidbody>();

        // Find these specific UI objects and assign them a variable
        deathScreen = canvas.transform.Find("Death Screen").gameObject;
        settings = canvas.transform.Find("Pause Setting Screen").gameObject;
        pause = canvas.transform.Find("PauseMenu").gameObject;
        displayDistance = canvas.transform.Find("Distance").gameObject.GetComponent<TextMeshProUGUI>();

        // Set the mouse sensitivity
        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 400); 

        // Set health to max
        currentHealth = maxHealth; 

        // If the health display is on, display it.
        if (healthDisplay != null){healthDisplay.text = "Health: " + currentHealth;}

        // Remove any equiped powerups
        ResetPowerup();

        //get capsule collider
        characterCollider = GetComponent<CapsuleCollider>();

        RefreshAdditionalBounces();
    }

    void Update() // Function Called once per frame
    {
        if (LaunchBrakingTimer > 0.0f)
        {
            LaunchBrakingTimer -= Time.deltaTime;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, hitLength + hitRadius))
        {
            hammerArcRoot.transform.position = hit.point;
        }
        else
        {
            hammerArcRoot.transform.position = cameraObject.transform.position + (cameraObject.transform.forward * hitLength);
        }

        /*for (int i = 0; i < hammerArcPoints.Length; i++)
        {
            if (i < hammerArcPoints.Length - 1)
            {
                Vector3 startLoc = hammerArcPoints[i].transform.position;
                Vector3 endLoc = hammerArcPoints[i + 1].transform.position;
                Debug.DrawLine(startLoc, endLoc, Color.green);
            }
        }*/

        //print(isLaunched);
        // Self Explanatory Functions
        HandleInput();
        HandleHammer();
        HandleLookRotation();
        HandleSpeedFX();

        // Record how long we are in the air, reset the timer while grounded
        if(!isGrounded)
        {
            hangTime += Time.deltaTime;
        }
        else
        {
            hangTime = 0;
        }

        // Set speedometer to players velocity
        speedometer.text = rb.velocity.magnitude.ToString("0.0") + "mph";
    }

    void FixedUpdate() // Function Called once per tick
    {
        // Shoot a raycast where the players looking, look for objects the player can bounce on
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out distanceCheck, 100000, bouncableLayers);

        // COMMENT: Self explanatory function calls. Should they be in here or Update() with the others?
        HandleMovement();
        UpdateDistanceHud();

        // if we just finished swinging the hammer...
        if (hammerHit)
        {
            // Tell game we arent in just swinging hammer state, and we haven't bounced off of a surface
            hammerHit = false;
            hammerBounced = false;
        }

        // If we just swiped our hammer (rmb) reset this variable. COMMENT: I think this could be implemented better
        if (hammerSwipe){hammerSwipe = false;}

        // If we are currently swinging our hammer, check to see if we bounced off something. COMMENT: I think this can be put elsewhere
        if (hittingHammer){HammerBounce();}

        // If we are currently swiping our hammer (rmb), see if we hit an enemy OR projectile
        if (swipingHammer){HammerHit();}
    }

    private void OnCollisionEnter(Collision other)
    {
        // If the player touches this object, they die [kill surface]
        if(other.gameObject.tag == "Kill")
        {
            Die();
        }

        if(other.gameObject.tag == "Heat")
        {
            TakeDamage(1);
            rb.AddForce(other.GetContact(0).normal * 25f, ForceMode.Impulse);
        }

        // If the player touches a moving platform, set the platform as the parent so the player moves with it
        if(other.gameObject.tag == "MovingPlatform")
        {
            transform.SetParent(other.gameObject.transform);
        }
    }
    private void OnCollisionExit(Collision other) {

        // If the player gets off a moving platform, remove the platform as the parent so the player doesnt move with it
        if(other.gameObject.tag == "MovingPlatform")
        {
            transform.SetParent(null);
        }
    }

    private void HandleSpeedFX() // Handles speed effects while moving quickly
    {
        if (rb.velocity.magnitude > 10)
        {
            speedlines.transform.LookAt(rb.velocity * 1000);
        }

        ParticleSystem.MainModule main = speed.main;
        ParticleSystem.EmissionModule emission = speed.emission;

        if (rb.velocity.magnitude > speedlineThreshold)
        {
            targetAlpha = (rb.velocity.magnitude - speedlineThreshold)/(speedlineMax - speedlineThreshold);
            targetSpeed = (rb.velocity.magnitude - speedlineThreshold)/(speedlineMax - speedlineThreshold) * maxParticleSpeed;
            targetEmission = (rb.velocity.magnitude - speedlineThreshold)/(speedlineMax - speedlineThreshold) * maxParticleEmission;
            targetZ = (rb.velocity.magnitude - speedlineThreshold)/(speedlineMax - speedlineThreshold) * maxParticleZ;

            if (rb.velocity.magnitude > fovThreshold)
            {
                targetFOV = ((rb.velocity.magnitude - fovThreshold)/(speedlineMax - fovThreshold) * maxAdditionalFOV) + initialFOV;
            }
        }
        else
        {
            targetAlpha = 0;
            targetSpeed = initialParticleSpeed;
            targetEmission = initialParticleEmission;
            targetZ = initialParticleZ;
            targetFOV = initialFOV;
        }

        main.startColor = new Color(1, 1, 1, Mathf.MoveTowards(main.startColor.color.a, targetAlpha, 100f * Time.deltaTime));
        main.startSpeed = Mathf.MoveTowards(main.startSpeed.constant, targetSpeed, 100f * Time.deltaTime);
        emission.rateOverTime = Mathf.MoveTowards(emission.rateOverTime.constant, targetEmission, 100f * Time.deltaTime);
        speed.gameObject.transform.localPosition = new Vector3(0, 0.5f, Mathf.MoveTowards(speed.gameObject.transform.localPosition.z, targetZ, 100f * Time.deltaTime));

        foreach (Camera cam in cameras)
        {
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, targetFOV, 100f * Time.deltaTime);
        }
    }

    private void HandleHammer() // Handles all hammer states and plays according animations.
    {
        if (secondaryPressed && !chargingHammer && !recovering && !hittingHammer && !hammerCharged && !swipingHammer && currentCombo == Combo.notSwiping)
        {
            swipingHammer = true;
            swipeComboReady = false;
            hammerTimer = swipeTime;
            anim.Play("Swipe Right", -1, 0.25f);
            currentCombo = Combo.Swipe1;
            hitDirection = Vector3.Normalize(new Vector3(Random.Range(-15f, -30f), Random.Range(-5.0f, 5.0f), Random.Range(0f, 10f)) + transform.forward);
            swingForce = swipeForceBase;
        }
        // combo swipes
        if (secondaryPressed && !chargingHammer && !recovering && !hittingHammer && !hammerCharged && !swipingHammer && swipeComboReady && currentCombo == Combo.Swipe1) // for a lil combo, might want to include input when swiping
        {
            swipingHammer = true;
            swipeComboReady = false;
            hammerTimer = swipeTime;
            anim.Play("Swipe Left", -1, 0.01f);
            currentCombo = Combo.Swipe2;
            hitDirection = Vector3.Normalize(new Vector3(Random.Range(15f, 30f), Random.Range(-5.0f, 5.0f), Random.Range(0f, 10f)) + transform.forward);
            swingForce = swipeForceBase;
        }
        if (secondaryPressed && !chargingHammer && !recovering && !hittingHammer && !hammerCharged && !swipingHammer && swipeComboReady && currentCombo == Combo.Swipe2)
        {
            swipingHammer = true;
            swipeComboReady = false;
            hammerTimer = swipeTime;
            anim.Play("Swipe Right", -1, 0.25f);
            currentCombo = Combo.Swipe1;
            hitDirection = Vector3.Normalize(new Vector3(Random.Range(-15f, -30f), Random.Range(-5.0f, 5.0f), Random.Range(0f, 10f)) + transform.forward);
            swingForce = swipeForceBase;
        }

        /*
        if (secondaryPressed && !chargingHammer && !recovering && !hittingHammer && !hammerCharged && !swipeRecovering && !swipingHammer && swipeComboReady && currentCombo == Combo.Swipe2)
        {
            Debug.Log("hammer swipe final");
            swipingHammer = true;
            swipeComboReady = false;
            hammerTimer = swipeTime;
            anim.Play("Hit 3");
            currentCombo = Combo.Swipe3;
        }
        */
        
        if (mousePressed && !chargingHammer && !recovering && !hittingHammer && !hammerCharged && !swipeRecovering && !swipingHammer)
        {
            //Debug.Log("hammer startin");
            chargingHammer = true;
            hammerTimer = chargeTime;
            //anim.Play("HammerCharge"); 
            anim.Play("Charge");
            hitDirection = transform.forward;
            hammerBounced = false;
        }

        if (chargingHammer && hammerTimer > 0.1 && mouseReleased)
        {
            anim.Play("Idle");
            currentCombo = Combo.notSwiping;
            chargingHammer = false;
            hammerTimer = 0;
            hammerBounced = false;
        }

        if (mouseReleased && hammerCharged)
        {
            hammerCharged = false;
            hittingHammer = true;
            //slamHitbox.ActivateCollider();
            hammerTimer = hitTime;
        }

        if (hammerTimer > 0)
        {
            hammerTimer -= Time.deltaTime;
        }
        else if (chargingHammer)
        {
            //Debug.Log("charging hammer ended");
            hammerCharged = true;
            chargingHammer = false;
            //anim.Play("HammerHold"); 
            anim.Play("Charged");
            hammerBounced = false;
        }
        else if (hittingHammer)
        {
            //Debug.Log("hammer hitting ended");
            hammerHit = true;
            hittingHammer = false;
            //audioManager.PlaySFX(audioManager.hit);
            //anim.Play("HammerHit"); 
            anim.Play("Slam");
            //slamHitbox.DeactivateCollider();

            recovering = true;
            hammerTimer = recoveryTime;
            hammerBounced = false;
            hitDirection = transform.forward;
            // hitDirection = Vector3.left + transform.forward;
            swingForce = smashForceBase;
        }
        else if (recovering)
        {
            //Debug.Log("recovery ended");
            recovering = false;
            hammerBounced = false;
        }
        else if (swipingHammer)
        {
            // Debug.Log("SWIPE OVER");
            hammerSwipe = true;
            swipingHammer = false;

            hammerSwung = false;

            swipeRecovering = true;
            hammerTimer = swipeRecoveryTime;
        }
        else if (swipeRecovering)
        {
            swipeRecovering = false;
            swipeComboReady = true;
            hammerTimer = swipeRecoveryTime;
        }
        else if (swipeComboReady)
        {
            swipeComboReady = false;
            currentCombo = Combo.notSwiping;
        }
    }

    private void HandleLookRotation() // Handles camera movement from mouse
    {
        //rotating player body left and right
        yRotation += mouseInputVector.x;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, yRotation, 0), 20);

        //rotating camera up and down
        xRotation -= mouseInputVector.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp the rotation to prevent flipping
        cameraObject.transform.localRotation = Quaternion.RotateTowards(cameraObject.transform.localRotation, Quaternion.Euler(xRotation, 0, 0), 20);
    }

    private void HandleInput() // Handles all inputs
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSensitivity;

        mouseInputVector = new Vector2(mouseX, mouseY);
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");;
        float verticalInput = Input.GetAxisRaw("Vertical");;

        Vector3 prevMovementVector = movementInputVector;
        movementInputVector = ((horizontalInput * transform.right) + (verticalInput * transform.forward)).normalized;

        if (prevMovementVector != movementInputVector)
        {
            isChangingDirection = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))// If the player has pressed the space key...
        {
            jumpPressed = true; // let the engine know jump was pressed
        }

        if (Input.GetKeyUp(KeyCode.Space)) // if the player releases the jump button...
        {
            // reset these variable to how they were before they pressed the jump button.
            jumpPressed = false;
            mustReleaseJump = false;
            jumpHoldChecking = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mousePressed = true;
            mouseReleased = false;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            mouseReleased = true;
            mousePressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            secondaryPressed = true;
            secondaryReleased = false;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            secondaryReleased = true;
            secondaryPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && pause.activeSelf == false && settings.activeSelf == false && alive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            pause.SetActive(true);

            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && (pause.activeSelf == true || settings.activeSelf == true))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            pause.SetActive(false);
            settings.SetActive(false);

            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.Play("Equip");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ctrlPressed = true;
            ctrlReleased = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ctrlPressed = false;
            ctrlReleased = true;
        }

        //all upcoming code could be put somewhere way better or in a function but its cool ok lol
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

        cameraHolder.transform.localRotation = Quaternion.RotateTowards(cameraHolder.transform.localRotation, Quaternion.Euler(0, 0, zCamRotate), camRotateSpeed * Time.deltaTime);
    }

    private void HandleMovement() // Handles all Player Rigidbody movements apart from hammer launches
    {

        #region Raycast Checks
        //all this raycast slope stuff is getting kinda out of hand
        // i agree -other programmer
        RaycastHit hit;
        Vector3 movementPlane;
        float slopeAngle = 0;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, playerHeight/2 + groundCheckDist, groundLayers)) //if on the ground
        {
            slopeAngle = Vector3.Angle(hit.transform.up, hit.normal);
            //Debug.Log(slopeAngle);
            if (slopeAngle > 1 && slopeAngle < maxSlopeAngle)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
            }

            if (isGrounded == false && hangTime >= .25)
            {
                audioManager.PlaySFX(audioManager.land);
                // Shake the screen when we land after being launched
                /*
                if (isLaunched)
                {
                    StartCoroutine(FindObjectOfType<ScreenShaker>().Shake(0.1f, 0.01f, 0, 0, 0.1f));
                }
                */

                if (!hammerCharged)
                {
                    anim.Play("Land");
                }
            }
            isGrounded = true;
            isLaunched = false;
            rb.useGravity = false;
            anim.SetBool("grounded", true);
            movementPlane = hit.normal;

            StopCoroutine(DecreaseCoyoteTime()); // Stop the coroutine that lets us have jump leinency
            hasCoyoteTime = true; // Reset our coyote time
            hasJumped = false; // Reset our jump tracker. 

            //Debug.Log(Vector3.Angle(hit.transform.up, hit.normal));
        }
        else // if not on the ground
        {
            movementPlane = transform.up;
            isGrounded = false;
            rb.useGravity = true;
            anim.SetBool("grounded", false);
            isOnSlope = false;

            // Let the player jump until this coroutine is finished.
            StartCoroutine(DecreaseCoyoteTime());

        }

        #endregion

        #region Ground Movement
        
        if (ctrlPressed && !isSliding) //might need to add additional requirements to this for if we, say, dont want to be able to slide during certain powerups
        {
            isSliding = true;
            characterCollider.height = slideHitboxHeight;
            characterCollider.center = new Vector3(0, -1 * ((2 - slideHitboxHeight)/2), 0);
            cameraHolder.transform.position = new Vector3(cameraHolder.transform.position.x, cameraHolder.transform.position.y - slideCameraOffset, cameraHolder.transform.position.z);
        }
        else if (ctrlReleased && isSliding)
        {
            isSliding = false;
            characterCollider.height = 2;
            characterCollider.center = new Vector3(0, 0, 0);
            cameraHolder.transform.position = new Vector3(cameraHolder.transform.position.x, cameraHolder.transform.position.y + slideCameraOffset, cameraHolder.transform.position.z);
        }

        Vector3 flatVelocity;

        if (isOnSlope)
        {
            movementInputVector = Vector3.ProjectOnPlane(movementInputVector, movementPlane);
        }

        flatVelocity = Vector3.ProjectOnPlane(rb.velocity, movementPlane);
        #endregion
        if (!isLaunched) // if we are not launched
        {
            if (isGrounded) // if were on the ground and not launched
            {
                //Debug.Log("is grounded: " + isGrounded);
                #region Jump
                if (jumpPressed && isGrounded && !hasJumped || jumpPressed && !isGrounded && hasCoyoteTime == true && !hasJumped)
                {
                    srcJumpPoint = rb.transform.position.y;
                    Jump();
                }
                #endregion

                // Debug.Log(flatVelocity);
                // Debug.Log("Velo: " + (flatVelocity + transform.forward));
                // Debug.Log("Velo Magnitude: " + flatVelocity.magnitude);
                anim.SetFloat("Speed", flatVelocity.magnitude);
                //Debug.Log(anim.GetFloat("Speed"));

                if (!isSliding)
                {
                    if (movementInputVector.magnitude > 0.001)
                    {
                        walkTime += 1;
                        if (walkTime%15 == 0)
                        {
                            audioManager.PlayWalk();
                            walkTime = 0;
                        }
                        // add some anims for changing direction, or move arms in direction of movement? (kaelen idea)
                        if (flatVelocity.magnitude < maxSpeed || flatVelocity.magnitude >= maxSpeed && isChangingDirection)
                        {
                            isChangingDirection = false;
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
                            walkTime = 0;
                        }
                        else
                        {
                            rb.velocity = new Vector3(0, rb.velocity.y, 0); //Vector3.ProjectOnPlane(new Vector3(0, rb.velocity.y, 0), movementPlane);
                        }
                    }
                }
                else
                {
                    if (movementInputVector.magnitude > 0.001)
                    {
                        //adapt the launched air turning code to turning the slide
                    }
                    else
                    {
                        if (flatVelocity.magnitude > minSlideSpeed)
                        {
                            if (slopeAngle == 0)
                            {
                                rb.AddForce(-flatVelocity * flatSlideDrag);
                            }
                            else if (slopeAngle > 0 && slopeAngle < maxSlopeAngle)
                            {
                                rb.AddForce(flatVelocity * ((slopeAngle/maxSlopeAngle) * maxSlideAcceleration));
                            }
                        }
                        else
                        {
                            //isSliding = false;
                            //add other necessary stuff to end the slide properly without falling through floor
                        }
                    }
                }
            }

            #region Air movement
            else // if we're in the air but we arent launched from a hammer bounce
            {
                if (hammerCharged && !isGrounded && hangTime > 1) // if the hammer is charged in the air for awhile pull us downward
                {
                    rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0));
                }
                /*
                if (rb.velocity.y > 0) // if were are rising in the air from hammer bounce
                {
                    rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0)); // add a force to pull us downward
                }
                */
                else if (rb.velocity.y > -1 && rb.velocity.y < 1) // if we are at the apex of our air height
                {
                    rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0));
                }
                else // if were falling
                {
                    rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0));
                    //rb.velocity = new Vector3(rb.velocity.x, -maxFallingSpeed, rb.velocity.z);
                }                

                if (movementInputVector.magnitude > 0.001)
                {
                    rb.AddForce(movementInputVector * airAccelerationRate);
                    rb.velocity = new Vector3(Vector3.ClampMagnitude(flatVelocity, airMaxSpeed).x, rb.velocity.y, Vector3.ClampMagnitude(flatVelocity, airMaxSpeed).z);
                }
            }

            anim.SetBool("Moving", CheckMoving());

            #endregion


        }
        else // if we are launched
        {
            // clamp the max speed we can actually go, may need to be changed for powerups and such
            rb.velocity = new Vector3(Vector3.ClampMagnitude(rb.velocity, airMaxHammerSpeed).x, rb.velocity.y, Vector3.ClampMagnitude(rb.velocity, airMaxHammerSpeed).z);

            if (hammerCharged && !isGrounded && hangTime > 1) // if the hammer is charged in the air for awhile pull us downward
            {
                rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0)); // drag us down as long as its charged
            }

            if (rb.velocity.y > 0) // if were are rising in the air from hammer bounce
            {
                rb.AddForce(new Vector3(0, -bounceGravity, 0)); // add a force to pull us downward
                //rb.AddForce(new Vector3(-bounceGravity, -bounceGravity, -bounceGravity)); // add a force to pull us downward
            }
            else if (rb.velocity.y > -1 && rb.velocity.y < 1) // if we are at the apex of our air height
            {
                rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0)); // apply a force for a moment to pull us down
            }
            else // if were falling
            {
                //rb.velocity = new Vector3(rb.velocity.x, -maxFallingSpeed, rb.velocity.z);
                rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0));
            }

            if (movementInputVector.magnitude > 0.001)
            {
                Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                float dotProduct = Vector3.Dot(movementInputVector, horizontalVelocity.normalized);
                if (dotProduct < 0)
                {
                    //Debug.Log("Against");
                    if (LaunchBrakingTimer <= 0.0f)
                    {
                        rb.AddForce(movementInputVector * airBrakingAccelerationRate);
                    }
                }
                else
                {
                    //Debug.Log("Toward");
                    float adjustedForce = Mathf.Lerp(airAccelerationRate, 0, (horizontalVelocity.magnitude / airMaxSpeed));
                    rb.AddForce(movementInputVector * adjustedForce);
                }
            }
        }
    }

    private IEnumerator DecreaseCoyoteTime(){ // This coroutine decreases jump coyote time and doesnt let the player jump once its done running 
        if(!decreasingCoyoteTime && hasCoyoteTime) // if we have coyote time and we aren't decreasing it yet
        {
            decreasingCoyoteTime = true; // let the coroutine know we are decreasing coyote time. this makes the coroutine only run when needed.
            yield return new WaitForSeconds(coyoteTime); // wait for the desired amount of coyote time desired.
            hasCoyoteTime = false; // after waiting for our window, let the engine know we missed our window
            decreasingCoyoteTime = false; // let the engine know the coroutine is done.
        }
    }

    private void Jump() // Is called when the player tried and is allowed to jump
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //remove our falling velocity so our jump doesnt have to fight gravity.
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // add a force upward
        hasJumped = true; // let the engine know we have jumped.
        if (!hammerCharged) {
            anim.Play("Jump");
        }
    }


    private void HammerBounce() // Checks if we have bounced off a surface, if so apply physics or hurt enemys
    {
        onHammerBounce.Invoke();

        if (hammerBounced)
        {
            return;
        }

        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        //Ray ray = gameCamera.ScreenPointToRay(impactPoint.transform.position);
        bool bouncy = false;

        RaycastHit[] hits = Physics.SphereCastAll(ray, hitRadius, hitLength, bouncableLayers, QueryTriggerInteraction.Collide);

        if (hits.Length > 0 || currentPowerup == Powerup.Airburst || additionalBounces > 0)
        {
            if (additionalBounces > 0 && !isGrounded)
            {
                onExtraHammerHit.Invoke();
                additionalBounces--;
            }
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.GetComponent<Renderer>() != null && hit.transform.gameObject.GetComponent<Renderer>().sharedMaterial.name == "ShockAbsorbMat")
                {
                    return;
                }

                if (hit.transform.gameObject.tag == "Bouncy")
                {
                    bouncy = true;
                }

                if (hit.transform.gameObject.tag == "End Platform")
                {
                    rb.AddForce(hit.transform.up * 50f, ForceMode.Impulse);
                    GameObject.Find("EndPlatform").GetComponent<EndPlatform>().triggerPlatform();
                }

                /* This is for the kill surface. makes it so if you even try to bounce off of it, you die.
                // If the player tries to bounce off of this surface, they die [kill surface]
                if (hit.transform.gameObject.tag == "Kill")
                {
                    Die();
                }
                */

                else if (hit.transform.gameObject.tag == "Enemy Flyer")
                {
                    var e = hit.transform.gameObject.GetComponent<EnemyFlyerController>();
                    e.TakeDamage(1, hitDirection, swingForce * 1.5f);
                }
                else if (hit.transform.gameObject.tag == "Enemy Shooter")
                {
                    var e = hit.transform.gameObject.GetComponent<EnemyShooterController>();
                    e.TakeDamage(1, hitDirection, swingForce * 1.5f);
                }
                else if (hit.transform.gameObject.tag == "Collectible" || hit.transform.gameObject.layer == 11) // 11 == gibs layer
                {
                    hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(ray.direction * 50f, ForceMode.Impulse);
                }
                else if (hit.transform.gameObject.tag == "Switch")
                {
                    hit.transform.gameObject.GetComponent<Switch>().SwapStateActive();
                }
            }

            //Vector3 normal = hit1.normal.normalized;
            //float angle = Vector3.Angle(hit1.point - cameraObject.transform.position, -transform.up);
            //float wallAngle = Vector3.Angle(normal, Vector3.down);
            //float wallVSFlatVelAngle = Vector3.Angle(normal, rb.velocity);

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //Vector3 launchDirection = (-(impactPoint.transform.position - transform.position)).normalized;
            Vector3 launchDirection = (-ray.direction).normalized;


            RaycastHit arcHit;
            for (int i = 0; i < hammerArcPoints.Length; i++)
            {
                if (i < hammerArcPoints.Length - 1)
                {
                    Vector3 startLoc = hammerArcPoints[i].transform.position;
                    Vector3 endLoc = hammerArcPoints[i + 1].transform.position;
                    Vector3 direction = (endLoc - startLoc).normalized;
                    float distance = Vector3.Distance(startLoc, endLoc);                 

                    if (Physics.Raycast(startLoc, direction, out arcHit, distance))
                    {
                        Debug.Log("hit");
                        //Debug.DrawLine(startLoc, endLoc, Color.red, 2.0f);
                        launchDirection = -direction;
                        break;
                    }
                    else
                    {
                        //Debug.DrawLine(startLoc, endLoc, Color.green);
                    }
                }
            }

            //Vector3 launchDirection = (-ray.direction).normalized;

            // Momentum Resetting - If the player is slamming in the opposite direction they're moving, we add some extra force so their velocity isn't just canceled out
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float dotProduct = Vector3.Dot(launchDirection, horizontalVelocity.normalized);
            if (dotProduct < 0)
            {
                rb.AddForce(-horizontalVelocity, ForceMode.VelocityChange);
            }

            if (movementInputVector.magnitude > 0.001)
            {
                Vector3 baseLaunchDirection = launchDirection;
                Vector3 modifiedDirection;
                if (dotProduct < 0)
                {
                    modifiedDirection = (0.9f * baseLaunchDirection) + (0.1f * movementInputVector);
                    //Debug.Log("Against");
                }
                else
                {
                    modifiedDirection = (0.7f * baseLaunchDirection) + (0.3f * movementInputVector);
                    //Debug.Log("Toward");
                }

                launchDirection = new Vector3(modifiedDirection.x, baseLaunchDirection.y, modifiedDirection.z);
            }

            if (currentPowerup == Powerup.Airburst)
            {
                ResetPowerup();
            }

            if (currentPowerup == Powerup.Explosive)
            {
                isLaunched = true;
                rb.AddForce(launchDirection * explosiveForce, ForceMode.Impulse);
            }
            else if (bouncy)
            {
                isLaunched = true;
                rb.velocity = Vector3.zero;
                rb.AddForce(launchDirection * bouncyForce/* + normal * 10*/, ForceMode.Impulse);
                rb.AddForce(transform.up * bouncyUpForce, ForceMode.Impulse);
            }

            Vector3 force = launchDirection; // the direction we will be applying force to the player
            if (isLaunched == false) // if we havent launched yet
            {
                // add magnitude to our force direction
                force = new Vector3(force.x * initialBounceForce, force.y * initialBounceForceY, force.z * initialBounceForce);

                // stop factoring in movement for first bounce by counteracting our WASD movement forces
                rb.AddForce(new Vector3(-rb.velocity.x, -rb.velocity.y, -rb.velocity.z), ForceMode.Impulse); 
            }
            else // if we have been launched by a previous bounce
            {
                // add magnitude to our force direction
                force = new Vector3(force.x * bounceForce, force.y * bounceForceY, force.z * bounceForce);
            }


            rb.AddForce(force, ForceMode.Impulse); // apply the force vector to the player *make them bounce*

            isLaunched = true; // set is launched to true
            hammerBounced = true; // let the engine know we bounced

            LaunchBrakingTimer = LaunchBrakingDelay;

            Instantiate(HammerSound, gameObject.transform.position, Quaternion.identity);
            StartCoroutine(FindObjectOfType<ScreenShaker>().Shake(0.25f, 0.01f, 0, 0, 0.25f));
            

            // bouncing up one wall over and over again is still far too viable, but theres some improvement to the basic 90 degree angled hammer wall bounces
            // if (angle < 110 && angle > 30 && wallAngle > 80 && wallAngle < 100)
            // {
            //     rb.AddForce(transform.up * 10, ForceMode.Impulse);
            // }

            // if (wallVSFlatVelAngle > 140)
            // {
            //     rb.velocity = new Vector3(0, rb.velocity.y, 0);
            // }

            if (currentPowerup == Powerup.Explosive)
            {
                LoseExplosive();
            }

            
        }
        else 
        {
            //audioManager.PlaySFX(audioManager.whiff);
        }
    }

    bool hammerSwung = false;
    
    private void HammerHit() // See if we it an enemy or projectile. Respond accordingly.
    {

        if (hammerSwung)
        {
            return;
        }

        //Add parry and hit sounds in if statements
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.SphereCastAll(ray, hitRadius, hitLength, swipeLayers);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                //Debug.Log(hit.collider.gameObject);
                if (hit.transform.gameObject.tag == "Enemy Flyer")
                {
                    Debug.Log("CALL ME ;)");
                    hit.transform.gameObject.GetComponent<EnemyFlyerController>().TakeDamage(1, hitDirection, swingForce);
                    Debug.Log(hitDirection);
                }
                else if (hit.transform.gameObject.tag == "Enemy Shooter")
                {
                    hit.transform.gameObject.GetComponent<EnemyShooterController>().TakeDamage(1, hitDirection, swingForce);
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Projectile"))
                {
                    Projectile projectile = hit.transform.gameObject.GetComponent<Projectile>();

                    if (!projectile.CheckParried())
                    {
                        projectile.initializeProjectile(ray.GetPoint(100), parriedProjectileSpeed, parriedProjectileLifetime, true, CalculateTargetEnemy(ray));
                        FindObjectOfType<Hitstop>().Stop(0.15f);
                    }
                }
                else if (hit.transform.gameObject.tag == "Collectible" || hit.transform.gameObject.layer == 11) // 11 == gibs layer
                {
                    hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(ray.direction * 50f, ForceMode.Impulse);
                }
                else if (hit.transform.gameObject.tag == "Switch")
                {
                    hit.transform.gameObject.GetComponent<Switch>().SwapStateActive();
                }
            }

            hammerSwung = true;

            StartCoroutine(FindObjectOfType<ScreenShaker>().Shake(0.1f, 0.01f, 0, 0, 0.1f));
        }
    }

    private GameObject CalculateTargetEnemy(Ray mouseLookDirection)
    {
        GameObject targetCandidate = null;

        Collider[] enemies = Physics.OverlapSphere(transform.position, maxTargetDistance, enemyLayers);

        float targetCandidateAngle = 1000;

        foreach (Collider enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minTargetDistance)
            {
                continue;
            }

            float angle = Vector3.Angle(mouseLookDirection.direction, enemy.transform.position - transform.position);
            if (angle > maxTargetAngle)
            {
                continue;
            }

            Debug.Log(angle);

            if (targetCandidate != null)
            {
                if (targetCandidateAngle > angle)
                {
                    Debug.Log("targetfound");
                    targetCandidate = enemy.gameObject;
                    targetCandidateAngle = angle;
                }
            }
            else
            {
                Debug.Log("targetfound");
                targetCandidate = enemy.gameObject;
                targetCandidateAngle = angle;
            }
        }

        return targetCandidate;
    }

    void OnDrawGizmosSelected()// Draws selected Gizmos for testing (I think)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minTargetDistance);
        Gizmos.DrawWireSphere(transform.position, maxTargetDistance);
    }

    void UpdateDistanceHud() // Update UI to show how far away the player is from an object the crosshair is over
    {
        // Set the current distance to the distance of the object from the player minus our hitrange
        currentDistance = distanceCheck.distance - (hitLength + distanceCheckBuffer);

        // If object is too far away, update UI to be red and say "Infinite" for range.
        if(distanceCheck.distance == 0)
        {
            displayDistance.color = Color.red;
            displayDistance.text = "infinity";
        }
        else
        {
            // If the current object is in range, update distance text to be cyan and say in range
            if(currentDistance <= 0)
            {
                displayDistance.text = "in range";
                displayDistance.color = Color.cyan;
                isInRange = true;
            }
            else // If current object is out of range then say we arent in range, display how far it is and change the color to red.
            {
                displayDistance.text = (currentDistance).ToString("0.00m");
                displayDistance.color = Color.red;
                isInRange = false;
            }
        }

    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthDisplay != null)
        {
            healthDisplay.text = "Health: " + currentHealth;
        }
    }

    public void TakeDamage(int damage) // called when the player needs to take damage
    {
        StartCoroutine(FindObjectOfType<ScreenShaker>().Shake(0.1f, 0.01f, 0, 0, 0.1f));

        currentHealth -= damage;
        if (healthDisplay != null)
        {
            healthDisplay.text = "Health: " + currentHealth;
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void UpdateSpawn(Checkpoint check)
    {
        if (currentCheckpoint != null)
        {
            currentCheckpoint.DeactivateCheckpoint();
        }
        currentCheckpoint = check;
    }

    public void Die() // this function is called when the player dies
    {
        // Kill the player, activate the death screen UI, and reset the timescale.
        alive = false;
        deathScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResetPlayer() // this function resets the player fully
    {
        // Make sure the player is alive. If they have no checkpoints, simply reload the scene.
        alive = true;
        if (currentCheckpoint == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        // COMMENT: Code below should be in an else statement for clarity's sake methinks.

        //if the player has a checkpoint stored, remove it and get an updated one. COMMENT: Doesnt this do what the code above does?
        currentCheckpoint.ResetState();

        // Remove any velocity from player and set their position to their current checkpoint position
        rb.velocity = Vector3.zero;
        transform.position = currentCheckpoint.transform.GetChild(0).position;

        currentHealth = maxHealth;
        if (healthDisplay != null)
        {
            healthDisplay.text = "Health: " + currentHealth;
        }
    }

    public bool CheckMoving() // Check to see if the player is putting in any movement input
    {
        return movementInputVector.magnitude != 0;
    }

    #region Powerups
    public Powerup GetCurrentPowerup() {  return currentPowerup; }

    public void CollectPowerup(Powerup newPowerup) // Equips a new powerup to the player and updates UI to display equiped powerup
    {
        currentPowerup = newPowerup;
        if (currentPowerup == Powerup.Explosive)
        {
            tempPowerupUI.text = "Active Powerup: Explosive";
        }
        else if (currentPowerup == Powerup.Airburst)
        {
            tempPowerupUI.text = "Active Powerup: Airburst";
        }
    }

    void LoseExplosive()// Removes any equipped powerups COMMENT: Seems useless?
    {
        ResetPowerup();
    }

    void ResetPowerup() // Removes any equipped powerups
    {
        currentPowerup = Powerup.None;
        if (tempPowerupUI != null)
        {
            tempPowerupUI.text = "Active Powerup: None";
        }
    }

    //Used by powerups
    public void LaunchPlayer(float force)
    {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        rb.AddForce((-ray.direction).normalized * force, ForceMode.Impulse);
    }
    
    public void IncreaseInitialForce(float amount)
    {
        initialBounceForce += amount;
    }

    public void IncreaseAdditionalBounces(int amount)
    {
        Debug.Log(amount);
        additionalBounces += amount;
    }

    void RefreshAdditionalBounces()
    {
        additionalBounces = maxAdditionalBounces;
    }
    #endregion

}