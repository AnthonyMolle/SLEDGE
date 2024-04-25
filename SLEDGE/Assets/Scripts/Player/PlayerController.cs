using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Character Component References
    [Header("Character Component References")]
    [SerializeField] Camera gameCamera;
    Rigidbody rb; // parent rigidbody
    Animator anim; // parent animator
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

    bool mousePressed = false;
    bool mouseReleased = true;

    bool secondaryPressed = false;
    bool secondaryReleased = false;
    #endregion

    #region Health and Spawning
    [Header("Health and Spawning")]
    [SerializeField] Checkpoint firstCheckpoint;
    Transform currentSpawn;
    int currentSpawnIndex = 0;
    Checkpoint currentCheckpoint;
    
    [SerializeField] int maxHealth = 1;
    private int currentHealth = 1;
    bool alive = true;
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
    [SerializeField] float airMaxSpeed = 100f;

    [SerializeField] float naturalAdditionalFallingSpeed = 4f; //natural rate our player will fall after the apex of their falling height.
    #endregion

    #region Jump
    [Header("Jump")]
    bool jumpPressed = false;
    bool jumpHoldChecking = false;
    bool mustReleaseJump = false;

    [SerializeField] float srcJumpPoint = 0.0f;
    [SerializeField] float jumpHoldCheckWindow = 0.25f;
    [SerializeField] float jumpForce = 2f;
    [SerializeField] float coyoteTime = 0.25f;
    [SerializeField] bool hasCoyoteTime = true;// THIS IS THE DEFAULT VALUE OF COYOTETIME
    [SerializeField] bool decreasingCoyoteTime = false;
    [SerializeField] bool hasJumped = false;
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
    bool isInRange = false; // represents if we are in range to sue the hammer.
    int distCheck;
    #endregion

    #region Hammer
    [Header("Hammer")]
    [SerializeField] float bounceForce = 10f;
    [SerializeField] float hitLength = 5f;

    [SerializeField] float chargeTime = 1f;
    [SerializeField] float hitTime = 1f;
    [SerializeField] float recoveryTime = 1f;

    [SerializeField] float swipeTime = 0.5f;
    [SerializeField] float swipeRecoveryTime = 0.5f;

    [SerializeField] float launchedRotationSpeed = 0.02f;

    [SerializeField] LayerMask bouncableLayers;

    [SerializeField] LayerMask swipeLayers;

    RaycastHit distanceCheck;

    bool isLaunched = false;

    bool hittingHammer = false;
    bool chargingHammer = false;
    bool recovering = false;

    bool swipingHammer = false;
    bool swipeRecovering = false;

    bool hammerCharged = false;
    bool hammerHit = false;
    bool hammerSwipe = false;

    float hammerTimer = 0;
    float hangTime = 0;
    float walkTime = 0;

    [SerializeField] float parriedProjectileSpeed = 1f;
    [SerializeField] float parriedProjectileLifetime = 10f;

    [SerializeField] float hammerCoyoteTime = 0.25f; // coyote time of the hammer.
    [SerializeField] bool hammerHasCoyoteTime = false; // represents whether we have coyote time or not.
    [SerializeField] bool hammerDecreasingCoyoteTime = false; // represents whether we are decreasing coyote time or not.
    Vector3 lastHammerPos = new Vector3(0,0,0); // last saved hammer position.

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    #endregion

    #region UI

    public TextMeshProUGUI displayDistance;

    public Slider chargeSlider;

    [SerializeField] GameObject pause;

    [SerializeField] GameObject settings;

    [SerializeField] GameObject deathScreen;

    #endregion

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // lock the cursor to the center of the screen
        Cursor.visible = false; // make the cursor not visible
        chargeSlider.maxValue = chargeTime; // set the max value of the charge slider to chargetime
        chargeSlider.value = chargeSlider.minValue; //set the value of the charge slider to 0
        
        rb = GetComponent<Rigidbody>(); // get the rigidbody of the parent component
        anim = GetComponent<Animator>();// get the animator of the parent component

        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 400); // set the mouse sensitivity

        currentHealth = maxHealth; // set health to max

        currentCheckpoint = firstCheckpoint; //set the currentcheckpoint to the start of the level.
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleHammer();
        HandleLookRotation();
        if(!isGrounded)
        {
            hangTime += Time.deltaTime;
        }
        else
        {
            hangTime = 0;
        }
    }

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out distanceCheck, 100000, bouncableLayers);

        HandleMovement();
        UpdateDistanceHud();

        if (hammerHit)
        {
            hammerHit = false;
            HammerBounce();
        }

        if (hammerSwipe)
        {
            hammerSwipe = false;
            HammerHit();
        }
    }

    private void HandleHammer()
    {
        if (secondaryPressed && !chargingHammer && !recovering && !hittingHammer && !hammerCharged && !swipeRecovering && !swipingHammer)
        {
            Debug.Log("starting hammer swipe");

            swipingHammer = true;
            hammerTimer = swipeTime;
            anim.Play("Hit 1");
        }
        
        if (mousePressed && !chargingHammer && !recovering && !hittingHammer && !hammerCharged && !swipeRecovering && !swipingHammer)
        {
            //Debug.Log("hammer startin");
            chargingHammer = true;
            hammerTimer = chargeTime;
            anim.Play("Charge 1");
        }

        if (chargingHammer && hammerTimer > 0.1 && mouseReleased)
        {
            anim.Play("Idle 1");
            chargingHammer = false;
            hammerTimer = 0;
        }

        //if (mouseReleased){chargeSlider.value = chargeSlider.minValue;}
        // if (chargingHammer){chargeSlider.value = chargeSlider.maxValue - hammerTimer;}
        // if (!chargingHammer && hammerCharged){chargeSlider.value = chargeSlider.maxValue;}
        // if (!chargingHammer && !hammerCharged && chargeSlider.value != chargeSlider.minValue){chargeSlider.value = chargeSlider.minValue;}

        if (mouseReleased && hammerCharged)
        {
            hammerCharged = false;
            hittingHammer = true;

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
            anim.Play("Charge 1 Hold");
        }
        else if (hittingHammer)
        {
            //Debug.Log("hammer hitting ended");
            hammerHit = true;
            hittingHammer = false;
            //audioManager.PlaySFX(audioManager.hit);
            anim.Play("Slam 1");

            recovering = true;
            hammerTimer = recoveryTime;
        }
        else if (recovering)
        {
            //Debug.Log("recovery ended");
            recovering = false;
        }
        else if (swipingHammer)
        {
            hammerSwipe = true;
            swipingHammer = false;

            swipeRecovering = true;
            hammerTimer = swipeRecoveryTime;
        }
        else if (swipeRecovering)
        {
            swipeRecovering = false;
        }

        // if the hammer is currently charged AND we are in range, set the last hammer position to the current spot in range.
        // if we arent in range, call the ienumerator to reduce our coyote time.
        if(hammerCharged) // if the hammer is charged...
        {
            if(isInRange)// if we are in range...
            {
                // track the last in range pos & stop the coroutine to decrease coyote time. also give back the coyote time.
                lastHammerPos = distanceCheck.point;
                StopCoroutine(HammerDecreaseCoyoteTime());
                hammerHasCoyoteTime = true;
            }
            else // if we arent in range of anything, start to coroutine to decrease coyote time.
            {
                StartCoroutine(HammerDecreaseCoyoteTime());
            }
        }
    }

    private void HandleLookRotation()
    {
        //rotating player body left and right
        yRotation += mouseInputVector.x;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, yRotation, 0), 20);

        //rotating camera up and down
        xRotation -= mouseInputVector.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraObject.transform.localRotation = Quaternion.RotateTowards(cameraObject.transform.localRotation, Quaternion.Euler(xRotation, 0, 0), 20);
    }

    private void HandleInput()
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
            if(!mustReleaseJump) // and they haven't been holding the button down...
            {
                jumpPressed = true; // let the engine know jump was pressed
            }
            
        }
        if (Input.GetKey(KeyCode.Space)) // If the player might be holding the jump button down...
        {
            StartCoroutine(JumpHoldTimer());// Start a coroutine to check if they have been holding the button.
        }

        if (Input.GetKeyUp(KeyCode.Space)) // if the player releases the jump button...
        {
            StopCoroutine(JumpHoldTimer()); // stop checking to see if they are holding it.

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

        //all upcoming code could be put somewhere way better or in a function but its cool ok lol
        //float xCamRotate;
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
        
        /*
        // if (verticalInput > 0.4)
        // {
        //     xCamRotate = camRotateAmountFB;
        // }
        // else if (verticalInput < -0.4)
        // {
        //     xCamRotate = -camRotateAmountFB;
        // }
        // else
        // {
        //     xCamRotate = 0;
        // }
        */

        //MAKE IT SO FORWARD AND LEFT RIGHT CAMROTATION SPEEDS ARE DIFFERENT
        cameraHolder.transform.localRotation = Quaternion.RotateTowards(cameraHolder.transform.localRotation, Quaternion.Euler(0, 0, zCamRotate), camRotateSpeed * Time.deltaTime);
    }

    private void HandleMovement()
    {
        #region Raycast Checks
        //all this raycast slope stuff is getting kinda out of hand
        // i agree -other programmer
        RaycastHit hit;
        Vector3 movementPlane;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, playerHeight/2 + groundCheckDist, groundLayers)) //if on the ground
        {
            if (isGrounded == false && hangTime >= .25)
            {
                audioManager.PlaySFX(audioManager.land);
            }
            isGrounded = true;
            movementPlane = hit.normal;

            StopCoroutine(DecreaseCoyoteTime()); // Stop the coroutine that lets us have jump leinency
            hasCoyoteTime = true; // Reset our coyote time
            hasJumped = false; // Reset our jump tracker. 

            if (Vector3.Angle(hit.transform.up, hit.normal) > 1)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
            }
        }
        else // if not on the ground
        {
            movementPlane = transform.up;
            isGrounded = false;
            isOnSlope = false;

            // Let the player jump until this coroutine is finished.
            StartCoroutine(DecreaseCoyoteTime());

        }

        #endregion

        #region Ground Movement
        
        Vector3 flatVelocity;

        if (isOnSlope)
        {
            movementInputVector = Vector3.ProjectOnPlane(movementInputVector, movementPlane);
        }

        flatVelocity = Vector3.ProjectOnPlane(rb.velocity, movementPlane);
        #endregion
        if (!isLaunched)
        {
            if (isGrounded)
            {
                if (isLaunched)
                {
                    isLaunched = false;
                }

                if (movementInputVector.magnitude > 0.001)
                {
                    walkTime += 1;
                    if (walkTime >= 15)
                    {
                        audioManager.PlayWalk();
                        walkTime = 0;
                    }
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
                    }
                    else
                    {
                        rb.velocity = new Vector3(0, rb.velocity.y, 0); //Vector3.ProjectOnPlane(new Vector3(0, rb.velocity.y, 0), movementPlane);
                    }
                }
            }

            #region Air movement
            else // if we're in the air
            {
                if(rb.velocity.y <= 0)// if we are at the apex of our air height
                {
                    rb.AddForce(new Vector3(0, -naturalAdditionalFallingSpeed, 0));
                }

                if (movementInputVector.magnitude > 0.001)
                {
                    rb.AddForce(movementInputVector * airAccelerationRate);
                    rb.velocity = new Vector3 (Vector3.ClampMagnitude(flatVelocity, airMaxSpeed).x, rb.velocity.y, Vector3.ClampMagnitude(flatVelocity, airMaxSpeed).z);
                }
                else
                {
                    // if (flatVelocity.magnitude > 0.01)
                    // {
                    //     rb.AddForce(-flatVelocity * airDecelerationRate);
                    // }
                    // else
                    // {
                    //     rb.velocity = new Vector3(0, rb.velocity.y, 0); //Vector3.ProjectOnPlane(new Vector3(0, rb.velocity.y, 0), movementPlane);
                    // }
                }
            }
            #endregion

            #region Jump
            if (jumpPressed && isGrounded && !hasJumped || jumpPressed && !isGrounded && hasCoyoteTime == true && !hasJumped)
            {
                //Debug.Log("jump!");
                srcJumpPoint = rb.transform.position.y;
                Jump();
            }
            #endregion
        }

        else if (isGrounded && rb.velocity.y <= 0 || flatVelocity.magnitude <= airMaxSpeed) // can get grounded immediately after launching, should be a slight buffer to when "isgrounded" is activated again
        {
            isLaunched = false;
        }
        else
        {
            if (movementInputVector.magnitude > 0.001)
            {
                rb.velocity = Vector3.RotateTowards(rb.velocity, Quaternion.AngleAxis(Vector3.SignedAngle(flatVelocity, movementInputVector, transform.up), transform.up) * rb.velocity, launchedRotationSpeed/100, 0);
            }
        }
    }

    private IEnumerator DecreaseCoyoteTime(){ // this coroutine decreases jump coyote time and doesnt let the player jump once its done running 
        if(!decreasingCoyoteTime && hasCoyoteTime) // if we have coyote time and we aren't decreasing it yet
        {
            decreasingCoyoteTime = true; // let the coroutine know we are decreasing coyote time. this makes the coroutine only run when needed.
            yield return new WaitForSeconds(coyoteTime); // wait for the desired amount of coyote time desired.
            hasCoyoteTime = false; // after waiting for our window, let the engine know we missed our window
            decreasingCoyoteTime = false; // let the engine know the coroutine is done.
        }
    }

    private IEnumerator HammerDecreaseCoyoteTime()
    {
        if(!hammerDecreasingCoyoteTime && hammerHasCoyoteTime) // if we have coyote time and we aren't decreasing it yet
        {
            hammerDecreasingCoyoteTime = true; // let the coroutine know we are decreasing coyote time. this makes the coroutine only run when needed.
            yield return new WaitForSeconds(hammerCoyoteTime); //wait for the desired amount of coyote time desired.
            hammerHasCoyoteTime = false; // after waiting for our window, let the engine know we missed our window
            hammerDecreasingCoyoteTime = false; // let the engine know the coroutine is done.
        }
    }

    private IEnumerator JumpHoldTimer(){ // this coroutine checks to see if the player has been holding the jump button.
        if(!jumpHoldChecking && jumpPressed == true) // if the coroutine isnt already running, and the player is pressing the jump button.
        {
            jumpHoldChecking = true; // let the engine know we are running the coroutine
            yield return new WaitForSeconds(jumpHoldCheckWindow); // check if the player is holding jump for this long.
            // the following variable will be set given the player has not let go of the jump key.
            mustReleaseJump = true; // let the engine know they are holding jump.
            jumpPressed = false; // tell the engine they arent trying to jump.
            jumpHoldChecking = false; // stop running current instance of the coruoutine.
        }
    }

    private void Jump() // is called when the player tried and is allowed to jump
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //remove our falling velocity so our jump doesnt have to fight gravity.
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // add a force upward
        hasJumped = true; // let the engine know we have jumped.
    }

    private void HammerBounce()
    {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit1;
        if (Physics.Raycast(ray, out hit1, hitLength, bouncableLayers))
        {
            //FindObjectOfType<ScreenShaker>().Shake(100f, 100f);

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            Vector3 normal = hit1.normal;
            float angle = Vector3.Angle(hit1.point - cameraObject.transform.position, -transform.up);
            float wallAngle = Vector3.Angle(normal, Vector3.down);
            float wallVSFlatVelAngle = Vector3.Angle(normal, rb.velocity);

            Debug.Log(angle);

            //bouncing up one wall over and over again is still far too viable, but theres some improvement to the basic 90 degree angled hammer wall bounces
            if (angle < 110 && angle > 30 && wallAngle > 80 && wallAngle < 100)
            {
                rb.AddForce(transform.up * 10, ForceMode.Impulse);
            }

            if (wallVSFlatVelAngle > 140)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

            rb.AddForce((transform.position - hit1.point).normalized * bounceForce, ForceMode.Impulse);
            isLaunched = true;
            audioManager.PlaySFX(audioManager.hit);

            if (hit1.transform.gameObject.tag == "Enemy Flyer")
            {
                hit1.transform.gameObject.GetComponent<FlyingEnemy>().TakeDamage(1);
            }
            else if (hit1.transform.gameObject.tag == "Enemy Shooter")
            {
                hit1.transform.gameObject.GetComponent<ShooterEnemy>().TakeDamage(1);
            }
        }
        else if(hammerHasCoyoteTime) // if the hammer has coyote time && we are out of range on hammer bounce...
        {
            //NOTE THAT COYOTE TIME DOESNT ACCOUNT FOR BOUNCABLE LAYERS...
        }
    }

    private void HammerHit()
    {
        //Add parry and hit sounds in if statements
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit1;
        if (Physics.Raycast(ray, out hit1, hitLength, swipeLayers))
        {
            if (hit1.transform.gameObject.tag == "Enemy Flyer")
            {
                hit1.transform.gameObject.GetComponent<FlyingEnemy>().TakeDamage(1);
                audioManager.PlaySFX(audioManager.hit);
            }
            else if (hit1.transform.gameObject.tag == "Enemy Shooter")
            {
                hit1.transform.gameObject.GetComponent<ShooterEnemy>().TakeDamage(1);
                audioManager.PlaySFX(audioManager.hit);
            }
            else if (hit1.transform.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                hit1.transform.gameObject.GetComponent<Projectile>().initializeProjectile(ray.GetPoint(100), parriedProjectileSpeed, parriedProjectileLifetime, true);
            }
        }
    }

    void UpdateDistanceHud()
    {
        currentDistance = distanceCheck.distance - (hitLength + distanceCheckBuffer);
        //Debug.Log(distanceCheck.distance);
        

        if(distanceCheck.distance == 0)
        {
            displayDistance.color = Color.red;
            displayDistance.text = "infinity";
        }
        else
        {
            if(currentDistance <= 0)
            {
                displayDistance.text = "in range";
                displayDistance.color = Color.cyan;
                isInRange = true;
            }
            else
            {
                displayDistance.text = (currentDistance).ToString("0.00m");
                displayDistance.color = Color.red;
                isInRange = false;
            }
        }

    }

    public void TakeDamage(int damage) // called when the player needs to take damage
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateSpawn(int index, Checkpoint check)
    {
        if (index > currentSpawnIndex)
        {
            currentSpawnIndex = index;
            currentCheckpoint = check;
        }
    }

    public void Die() // this function is called when the player dies
    {
        Debug.Log("die!");
        alive = false;
        deathScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResetPlayer() // this function resets the player fully
    {
        alive = true;

        if (currentCheckpoint != null) //if the player has a checkpoint stored, remove it and get an updated one.
        {
            currentCheckpoint.Reset();
        }

        rb.velocity = Vector3.zero;

        transform.position = currentCheckpoint.transform.position; // set the player to the current check point pos.

        currentHealth = maxHealth;
    }
}
