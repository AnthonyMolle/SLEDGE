using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class EnemyFlyerController : EnemyBaseController
{
    
    
    private enum CombatState
    {
        IDLE,
        HUNTING,
        AIMING,
        ATTACKING,
        RECOVERING
    }

    CombatState combatState = CombatState.IDLE;

    float dashCooldown = 3;
    float cooldown;

    float dashRadius = 10;
    float dashSpeed = 30;

    float aimTimer;
    float attackTimer;
    float recoverTimer;

    Vector3 recoveryLocation;

    int currentPathIndex = -1;
    Vector3 currentPathPoint;
    bool UsingPath = false;

    Vector3 offset = new Vector3(0, 7, 0);
    Vector3 currentKnownTargetPos;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        cooldown = dashCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        // Update Timers
        switch (combatState)
        {
            case CombatState.AIMING:
                aimTimer += Time.deltaTime;
                break;
            case CombatState.ATTACKING:
                attackTimer += Time.deltaTime;
                break;
            case CombatState.RECOVERING:
                recoverTimer += Time.deltaTime;
                break;
            default:
                cooldown += Time.deltaTime;
                break;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log("Combat state: " + combatState);
        switch (enemyState)
        {
            case EnemyState.IDLE:

                combatState = CombatState.IDLE;
                if (PlayerinLOS())
                {
                    enemyState = EnemyState.HOSTILE;
                    break;
                }

                if (IsTargetDirectlyReachable(spawnPosition))
                {
                    MoveTowardsLocation(spawnPosition);
                }
                else
                {
                    rb.velocity = new Vector3(0, 0, 0);
                }

                break;

            case EnemyState.HOSTILE:

                switch (combatState)
                {
                    // ENEMY IS AWARE OF PLAYER PRESENCE, BUT HASN'T DONE ANYTHING
                    case CombatState.IDLE:

                        if (Vector3.Distance(transform.position, player.transform.position) <= dashRadius)
                        {
                            rb.velocity = new Vector3(0, 0, 0);
                            RotateTowardsTarget(player);

                            if (cooldown >= dashCooldown)
                            {
                                aimTimer = 0;
                                combatState = CombatState.AIMING;
                            }
                        }
                        else
                        {
                            // Begin moving toward player
                            combatState = CombatState.HUNTING;
                            Debug.Log("HUNTING...");
                            MoveTowardTarget(player.transform.position);
                        }
                        break;

                    // ENEMY IS MOVING INTO ATTACK RANGE
                    case CombatState.HUNTING:

                        if (Vector3.Distance(transform.position, player.transform.position) <= dashRadius)
                        {
                            rb.velocity = new Vector3(0, 0, 0);
                            RotateTowardsTarget(player);

                            if (cooldown >= dashCooldown)
                            {
                                aimTimer = 0;
                                combatState = CombatState.AIMING;
                            }

                            Debug.Log("In range!");
                        }
                        else
                        {
                            // Continue moving toward player
                            MoveTowardTarget(player.transform.position);
                        }

                        break;

                    // ENEMY IS PREPARING TO DASH
                    case CombatState.AIMING:

                        RotateTowardsTarget(player);

                        if (aimTimer >= 1.0f)
                        {
                            Debug.Log("attack!");
                            attackTimer = 0;
                            combatState = CombatState.ATTACKING;
                            DashForward();
                        }
                        // Update aim timer, and if we have aimed long enough, begin dash
                        break;

                    // ENEMY IS ATTACKING
                    case CombatState.ATTACKING:

                        // If we reached maximum time dashing, return to idle combat state
                        if (attackTimer >= 1.5)
                        {
                            cooldown = 0;
                            rb.velocity = new Vector3(0, 0, 0);
                            recoveryLocation = transform.position + new Vector3(0, 8, 0);
                            combatState = CombatState.RECOVERING;
                        }

                        break;
                    case CombatState.RECOVERING:

                        if (recoverTimer >= 2.0)
                        {
                            recoverTimer = 0;
                            rb.velocity = new Vector3(0, 0, 0);
                            combatState = CombatState.IDLE;
                            break;
                        }

                        if (Vector3.Distance(transform.position, recoveryLocation) >= 1)
                        {
                            MoveTowardsLocation(recoveryLocation);
                        }
                        else
                        {
                            rb.velocity = new Vector3(0, 0, 0);
                            RotateTowardsTarget(player);
                        }

                        break;
                }
                
                if (!PlayerinLOS())
                {
                    Debug.Log("not in LOS");
                    enemyState = EnemyState.IDLE;
                }

                break;

            case EnemyState.DEAD:

                break;

            default:

                break;
        }
    }

    void RotateTowardsTarget(GameObject target)
    {
        var targetRotation = Quaternion.LookRotation((target.transform.position) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    void DashForward()
    {
        rb.velocity = transform.forward * dashSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (combatState == CombatState.ATTACKING)
        {
            Debug.Log("hit");
            cooldown = 0;
            rb.AddForce(transform.forward * -10, ForceMode.Impulse);
            recoveryLocation = transform.position + new Vector3(0, 8, 0);
            combatState = CombatState.RECOVERING;

            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("KILLL!!!!!");
                other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                TakeDamage(1, transform.forward * -1, 10f);
            }
        }
    }

    #region Movement/Pathing

    // Enemy moves toward player, either directly or by finding a path if there is an obstruction between them
    void MoveTowardTarget(Vector3 target)
    {
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
    }

    // Sets up a path from the enemy to the player to navigate
    void SetUpPathing()
    {
        currentPathIndex = PlayerTracker.getPathIndex(transform.position);
        currentPathPoint = PlayerTracker.getPointFromIndex(currentPathIndex);
        GetComponent<BoxCollider>().enabled = false;
        UsingPath = true;
    }

    // Moves the enemy along a path toward the player
    void followPathToTarget()
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

    // Moves and rotate the enemy toward a specific location
    void MoveTowardsLocation(Vector3 location)
    {
        Vector3 targetDirection = (location - transform.position).normalized;

        rb.velocity = targetDirection * movementSpeed;

        var targetRotation = Quaternion.LookRotation((location) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    // Check if enemy can reach player without pathing, by checking if all four corners of the enemy can successfully raycast the player
    // 
    // Note: This can definitely just use a boxcast, which would be more readable and reliable, so we should implement that in the future (not an immediate issue, gets the job done)
    private bool IsTargetDirectlyReachable(Vector3 target)
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
                /*if (hit.transform != target)
                {
                    return false;
                }*/
            }
        }

        return true;
    }

    #endregion

}
