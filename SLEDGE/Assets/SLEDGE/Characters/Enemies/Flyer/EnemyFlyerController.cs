using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Splines;
using UnityEngine.VFX;
using static UnityEditor.FilePathAttribute;

public class EnemyFlyerController : EnemyBaseController
{

    [Header("Flyer Stats")]
    [HorizontalLine]

    [SerializeField] float dashCooldown = 3;        // How long the flyer will take before dashing again
    [SerializeField] float dashRadius = 10;         // Distance from the player the flyer will begin dashing at
    [SerializeField] float dashSpeed = 30;          // Speed of the flyer while dashing

    [SerializeField] float aimDuration = 1.0f;      // Duration flyer charges attack
    [SerializeField] float attackDuration = 1.5f;   // Duration flyer dashes
    [SerializeField] float recoverDuration = 2.0f;  // Duration flyer recovers after dashing

    [SerializeField] GameObject shockHitbox; // Hitbox of AOE attack
    Color shockHitboxColor; 
    bool playerInRadius = false; // Whether the player is in the AOE radius
    bool playerHit = false; // Whether the player has already been hit by the AOE attack this cycle
    Vector3 maxShockScale;

    [Header("VFX/SFX")]
    [HorizontalLine]

    [SerializeField] GameObject eyeLight;
    [SerializeField] AudioClip idleSound;
    [SerializeField] AudioClip telegraphSound;      // Sound the flyer makes while charging an attack
    AudioSource audioSource;

    /* Timers for tracking cooldowns and action durations */
    float cooldown;
    float aimTimer;
    float attackTimer;
    float recoverTimer;

    /* Pathing variables */
    int currentPathIndex = -1;
    Vector3 currentPathPoint;
    bool UsingPath = false;

    Vector3 offset = new Vector3(0, 7, 0);  // Offset of the player that the enemy moves toward

    Vector3 recoveryLocation;               // Where the flyer moves to when recovering from a dash

    Vector3 pathPosition;

    SplineAnimate splineComponent;

    Animator anim;

    // DEATH EXPLOSION STUFF
    float deathTimer = 0.0f;
    [SerializeField] float dyingDuration = 2.0f; // Duration between taking fatal damage and exploding
    bool launched = false; // If the enemy was launched by the player
    Vector3 launchDirection;
    public GameObject explosionEffect;
    bool explosionTriggered = false;

    [SerializeField] LayerMask enemyLayers;

    public Color defaultColor;
    public Color deathColor;

    private enum CombatState
    {
        IDLE,
        HUNTING,
        AIMING,
        ATTACKING,
        RECOVERING,
        DYING
    }

    CombatState combatState = CombatState.IDLE;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        cooldown = dashCooldown; // Since we don't want the enemy to spawn with its cooldowns up
        audioSource = GetComponent<AudioSource>();

        eyeLight.GetComponent<Light>().intensity = 0.0f;

        maxShockScale = shockHitbox.transform.localScale;

        shockHitboxColor = shockHitbox.GetComponent<MeshRenderer>().material.color;

        shockHitboxColor.a = 0.0f;
        shockHitbox.GetComponent<MeshRenderer>().material.color = shockHitboxColor;
        shockHitbox.GetComponent<MeshRenderer>().enabled = false;

        anim = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update() // NOTE: This could get moved to FixedUpdate in the future so there isn't two update functions,
                  // put this here cause I wasn't sure how deltatime interacted with fixedupdate but it seems to be fine to use there - Dom
    {
        // Update Timers
        switch (combatState)
        {
            case CombatState.AIMING:
                
                aimTimer += Time.deltaTime;
                
                float brightness = Mathf.Lerp(0.1f, 2.0f, aimTimer / aimDuration);    
                eyeLight.GetComponent<Light>().intensity = brightness; // Light telegraph based on how close to attacking the enemy is            

                Vector3 shockScale = Vector3.Lerp(Vector3.zero, maxShockScale, aimTimer / aimDuration);
                shockHitbox.transform.localScale = shockScale;

                break;
            
            case CombatState.ATTACKING:
                
                attackTimer += Time.deltaTime;             
                break;
            
            case CombatState.RECOVERING:
                
                recoverTimer += Time.deltaTime;
                break;

            case CombatState.DYING:
                
                deathTimer += Time.deltaTime;

                eyeLight.GetComponent<Light>().intensity = Mathf.Lerp(0.1f, 2.0f, (deathTimer % 0.25f) / 0.25f);

                break;

            default:
                
                cooldown += Time.deltaTime;
                break;
        
        }
    }

    private void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.IDLE:
                eyeLight.GetComponent<Light>().color = defaultColor;

                combatState = CombatState.IDLE;
                if (PlayerinLOS())
                {
                    enemyState = EnemyState.HOSTILE;
                }
                /*else if (Vector3.Distance(transform.position, spawnPosition) > 2 && IsTargetDirectlyReachable(spawnPosition))  // If we can't find player, try to go back to spawn (We can't path there because our pathing logic only works for the player)
                {
                    MoveTowardsLocation(spawnPosition);
                }
                else                                                // If we can't get back to spawn, stay in place (Not ideal, should set up pathing back to spawn eventually)
                {
                    rb.velocity = new Vector3(0, 0, 0);
                }*/

                break;

            case EnemyState.HOSTILE:

                switch (combatState)                                // Combat State tracks all of the flyer specific behaviors when engaged with the player
                {
                    case CombatState.IDLE:                          // IDLE: Enemy is aware of players presence, but hasn't done anything yet

                        if (Vector3.Distance(transform.position, player.transform.position) <= dashRadius)
                        {
                            TryStartingAttack();
                        }
                        else
                        {
                            combatState = CombatState.HUNTING;
                            MoveTowardTarget(player.transform.position);
                        }

                        break;

                    case CombatState.HUNTING:                       // HUNTING: Enemy is moving into attack range

                        if (Vector3.Distance(transform.position, player.transform.position) <= dashRadius)
                        {
                            TryStartingAttack();
                        }
                        else
                        {
                            MoveTowardTarget(player.transform.position);
                        }

                        break;

                    case CombatState.AIMING:                        // AIMING: Enemy is preparing to attack

                        RotateTowardsTarget(player.transform.position);

                        if (aimTimer >= aimDuration)
                        {
                            attackTimer = 0;

                            audioSource.clip = idleSound;
                            audioSource.Play();

                            combatState = CombatState.ATTACKING;
                            AreaBlast();
                        }

                        break;

                    case CombatState.ATTACKING:                     // ATTACKING: Enemy is attacking

                        if (attackTimer >= attackDuration)
                        {
                            rb.velocity = new Vector3(0, 0, 0);

                            BeginAttackRecovery();
                        }
                        else
                        {
                            RotateTowardsTarget(player.transform.position);
                        }

                        break;
                    case CombatState.RECOVERING:                    // RECOVERING: Enemy is recovering from their attack

                        if (recoverTimer >= recoverDuration)
                        {
                            recoverTimer = 0;

                            rb.velocity = new Vector3(0, 0, 0);

                            gameObject.GetComponent<SplineAnimate>().Play();

                            combatState = CombatState.IDLE;
                        }
                        else
                        {
                            RotateTowardsTarget(pathPosition);
                        }
                        /*else if (Vector3.Distance(transform.position, recoveryLocation) >= 1)
                        {
                            MoveTowardsLocation(recoveryLocation);
                        }
                        else
                        {
                            rb.velocity = new Vector3(0, 0, 0);
                            RotateTowardsTarget(player);
                        }*/

                        break;
                    case CombatState.DYING:

                        if (launched)
                        {
                            // Move
                            transform.position = transform.position + (launchDirection * 0.75f);
                        }
                        else if (deathTimer > (dyingDuration * 0.9) && !explosionTriggered)
                        {
                            explosionTriggered = true;
                            Instantiate(explosionEffect, transform.position, transform.rotation);
                        }
                        else if (deathTimer > dyingDuration)
                        {
                            // Blow up
                            TriggerDeathExplosion(false);
                        }
                        else
                        {
                            RotateTowardsTarget(player.transform.position);
                        }

                        break;
                }
                
                if (!PlayerinLOS())
                {
                    //enemyState = EnemyState.IDLE;
                }

                break;

            case EnemyState.DEAD:

                break;

            default:

                break;
        }
    }

    void AreaBlast()
    {
        // UPDATED SFX AND VFX FOR SHOCK ATTACK WOULD TRIGGER HERE

        shockHitboxColor.a = 0.3f;
        shockHitbox.GetComponent<MeshRenderer>().material.color = shockHitboxColor;

        if (playerInRadius)
        {
            shockPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (launched)
        {
            TriggerDeathExplosion(true);
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRadius = true;
        }

        if (playerInRadius == true && combatState == CombatState.ATTACKING && playerHit == false)
        {
            shockPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRadius = false;
        }
    }

    void shockPlayer()
    {
        Debug.Log("playerhit");
        player.GetComponent<PlayerController>().TakeDamage(1);
        playerHit = true;

        Vector3 direction = Vector3.Normalize(player.transform.position - gameObject.transform.position);
        player.GetComponent<Rigidbody>().AddForce(direction * 20, ForceMode.Impulse);
    }

    public override void TakeDamage(int damage, Vector3 direction, float force)
    {
        //base.TakeDamage(damage, direction, force);
        anim.Play("Hit");

        if (enemyState == EnemyState.DEAD || combatState == CombatState.DYING)
        {
            return;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Enter dying state
            StartDeathCountdown();
        }
    }

    void StartDeathCountdown()
    {
        gameObject.GetComponent<SplineAnimate>().Pause();

        anim.SetBool("Dying", true);

        shockHitbox.transform.localScale = maxShockScale;
        shockHitboxColor.a = 0.0f;
        shockHitbox.GetComponent<MeshRenderer>().material.color = shockHitboxColor;
        shockHitbox.GetComponent<MeshRenderer>().enabled = true;

        audioSource.clip = telegraphSound;
        audioSource.Play();

        eyeLight.GetComponent<Light>().color = deathColor;
        eyeLight.GetComponent<Light>().intensity = 1.0f;

        combatState = CombatState.DYING;
    }

    public bool InDyingState()
    {
        return combatState == CombatState.DYING;
    }    

    public void LaunchFlyer(Vector3 direction)
    {
        anim.Play("Hit");
        launched = true;
        launchDirection = direction;
    }

    public void TriggerDeathExplosion(bool playerTriggered)
    {
        //Instantiate(explosionEffect, transform.position, transform.rotation);

        if (playerTriggered)
        {
            RaycastHit[] enemyHits = Physics.SphereCastAll(transform.position, 5.0f, transform.forward, 0.1f, enemyLayers);
            if (enemyHits.Length > 0)
            {
                foreach (RaycastHit hit in enemyHits)
                {
                    if (hit.transform.GetComponent<EnemyBaseController>() != null)
                    {
                        hit.transform.GetComponent<EnemyBaseController>().TakeDamage(1, Vector3.zero, 0.0f);
                    }
                }
            }
        }
        else if (playerInRadius)
        {
            shockPlayer();
        }

        enemyState = EnemyState.DEAD;
        Die();
    }

    protected override void Die()
    {
        base.Die();

        // Destroy the spline container parent that holds flyer splines
        if (gameObject.GetComponent<SplineAnimate>().Container != null)
        {
            Destroy(gameObject.GetComponent<SplineAnimate>().Container.transform.parent.gameObject);
        }
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        explosionTriggered = false;
    }

    void TryStartingAttack() // Track player and if we are off cooldown, begin aiming our attack
    {
        //rb.velocity = new Vector3(0, 0, 0);
        //RotateTowardsTarget(player);

        if (cooldown >= dashCooldown)
        {
            audioSource.clip = telegraphSound;
            audioSource.Play();

            aimTimer = 0;
            shockHitbox.GetComponent<MeshRenderer>().enabled = true;
            
            pathPosition = gameObject.transform.position + (gameObject.transform.forward * 5);

            gameObject.GetComponent<SplineAnimate>().Pause();

            anim.SetBool("Target", true);
            anim.SetFloat("Charge", 1);

            combatState = CombatState.AIMING;
        }
    }
    void BeginAttackRecovery()
    {
        cooldown = 0;
        recoveryLocation = transform.position + new Vector3(0, 8, 0);

        playerHit = false;

        eyeLight.GetComponent<Light>().intensity = 0.0f;
        
        shockHitboxColor.a = 0.0f;
        shockHitbox.GetComponent<MeshRenderer>().material.color = shockHitboxColor;
        shockHitbox.GetComponent<MeshRenderer>().enabled = false;

        anim.SetBool("Target", false);

        //gameObject.GetComponent<SplineAnimate>().Play();

        combatState = CombatState.RECOVERING;
    }

    #region Movement/Pathing
    
    void RotateTowardsTarget(Vector3 target)
    {
        var targetRotation = Quaternion.LookRotation((target) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    void DashForward()
    {
        //rb.velocity = transform.forward * dashSpeed;
    }

    void MoveTowardTarget(Vector3 target)   // Enemy moves toward target, either directly or by finding a path if there is an obstruction between them (pathing only works for tracking player)
    {
        /*
        if (IsTargetDirectlyReachable(target))
        {
            // If we were pathing
            if (UsingPath)
            {
                UsingPath = false;
                GetComponent<BoxCollider>().enabled = true;
            }

            MoveTowardsLocation(target + offset);

        }
        else
        {
            if (UsingPath == false) SetUpPathing();
            followPathToTarget();
        }
        */
    }

    void SetUpPathing()                     // Sets up a path from the enemy to the player to navigate
    {
        currentPathIndex = PlayerTracker.getPathIndex(transform.position);
        currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
        GetComponent<BoxCollider>().enabled = false;
        UsingPath = true;
    }

    void followPathToTarget()               // Moves the enemy along a path toward the player
    {
        MoveTowardsLocation(currentPathPoint);

        if (Vector3.Distance(transform.position, currentPathPoint) < 1)
        {
            currentPathIndex += 1;

            bool pathFinished = PlayerTracker.getSize() <= currentPathIndex;

            if (pathFinished)
            {
                currentPathIndex = PlayerTracker.getPathIndex(transform.position);
            }

            currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
        }
    }

    void MoveTowardsLocation(Vector3 location) // Moves and rotate the enemy toward a specific location
    {
        /*
        Vector3 targetDirection = (location - transform.position).normalized;

        rb.velocity = targetDirection * movementSpeed;

        var targetRotation = Quaternion.LookRotation((location) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        */
    }

    private bool IsTargetDirectlyReachable(Vector3 target)  // Check if enemy can reach player without pathing, by checking if all four corners of the enemy can successfully raycast the player
                                                            // Note: This can definitely just use a boxcast, which would be more readable and reliable,
                                                            // so we should implement that in the future (not an immediate issue, gets the job done) - Dom
    {
        RaycastHit hit;

        // Raycasts all four corners of our object...
        // Ensures target is not only visible but reachable...
        // No blocks are blocking path!

        Vector3[] offsets = { Vector3.zero, Vector3.left, Vector3.right, Vector3.up, Vector3.down };

        foreach (Vector3 x in offsets)
        {
            Vector3 startPoint = transform.position + x;
            Vector3 dirToTarget = target - startPoint;

            if (Physics.Raycast(startPoint, dirToTarget, out hit, Mathf.Infinity, 1 << 0 | 1 << 12))
            {
                if (Vector3.Distance(hit.transform.position, target) >= 5)
                {
                    return false;
                }
            }
        }

        return true;
    }

    #endregion

}
