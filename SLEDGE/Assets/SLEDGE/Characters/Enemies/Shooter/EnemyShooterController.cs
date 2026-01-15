using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyShooterController : EnemyBaseController
{

    [Header("Shooter Stats")]
    [HorizontalLine]

    [SerializeField] GameObject projectileClass;

    /* Handling projectile instances */
    GameObject projectile;
    List<GameObject> projectiles = new List<GameObject>();

    /* Cooldown between shots */
    [SerializeField] float shootCooldown = 3.0f;
    [SerializeField] float shootBigCooldown = 3.0f;
    [SerializeField] bool shootBurst = true;
    [SerializeField] int shootBurstCount = 3;
    private int currentShootBurstCount;
    float cooldown;
    bool burstFiring = false;

    [SerializeField] float aimDuration = 2.0f;
    [SerializeField] float burstAimDuration = 2.0f;
    float aimTimer = 0.0f;

    /* Projectile stats */
    [SerializeField] float projectileLifetime;
    [SerializeField] float projectileSpeed;

    /* Animation Curve for aim verticality */
    //Aiming only takes the current velocity vector into account and assumes it won't change. Any jumping, hammering, falling, or other vertical movement will jack up the vertical calculations because those accelerations aren't accounted for
    //The animation curve assumes that the velocity's Y vector at any given moment will somewhat even out over time. Y+ velocity is countered by gravity, Y- velocity is countered by assuming the player has a plan outside of freefalling forever
    [Header("Aiming")]
    [HorizontalLine]
    [SerializeField] AnimationCurve verticalAimMod; //X is time for bullet to hit player (normalized to graphMax), Y is % of vertical distance change we use from the original calculations
    [SerializeField] float graphMax; //unit is seconds. Evaluating the graph at any time after this point will return a Y equal to y(1)

    [Header("Positional Constants")]
    [HorizontalLine]

    /* Used for rotating to face player */
    [SerializeField] GameObject lookAtTarget;

    [SerializeField] Transform gunPosition;
    [SerializeField] GameObject gunLight;

    [Header("Audio")]
    [HorizontalLine]

    [SerializeField] AudioClip shootSound;
    AudioSource audioSource;

    // ANIM/RAGDOLL STUFF
    Animator anim;
    float hitTimer = 0.5f;
    bool deathTriggered = false;
    CapsuleCollider hitbox;


    private enum CombatState
    {
        COOLDOWN,
        AIMING
    }
    
    CombatState combatState = CombatState.COOLDOWN;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        currentShootBurstCount = 0;
        cooldown = shootCooldown;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        hitbox = GetComponent<CapsuleCollider>();

        gunLight.GetComponent<Light>().intensity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (combatState == CombatState.AIMING && enemyState != EnemyState.DEAD)
        {
            aimTimer += Time.deltaTime;

            float brightness = Mathf.Lerp(0.1f, 2.0f, aimTimer / 2.0f);
            gunLight.GetComponent<Light>().intensity = brightness;
        }
        else
        {
            cooldown += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.IDLE: 
                
                if (PlayerinLOS())
                {
                    enemyState = EnemyState.HOSTILE;
                }
                break;
            
            case EnemyState.HOSTILE:
                
                LookAtTarget(player);
                
                switch(combatState)
                {
                    case CombatState.AIMING:
                        
                        if ( ((shootBurst && burstFiring && aimTimer >= burstAimDuration) || aimTimer >= aimDuration) && CollisionTime() > 0f) //Only attempts to shoot if they think they can hit the target
                        {
                            aimTimer = 0.0f;
                            combatState = CombatState.COOLDOWN;

                            audioSource.Stop();
                            gunLight.GetComponent<Light>().intensity = 0.0f;

                            burstFiring = false;
                            FireProjectile(AimAhead());
                            //FireProjectile(player.transform.position);
                        }

                        break;

                    case CombatState.COOLDOWN:
                    
                        if (PlayerinLOS() && cooldown >= shootCooldown)
                        {
                            if (shootBurst)
                            {
                                if (currentShootBurstCount <= 0)
                                {
                                    if(cooldown >= shootBigCooldown)
                                    {
                                        cooldown = 0.0f;
                                        audioSource.time = 0.0f;
                                        audioSource.Play();
                                        currentShootBurstCount = shootBurstCount;
                                        combatState = CombatState.AIMING;
                                        currentShootBurstCount--;
                                    }
                                    else
                                    {
                                        burstFiring = false;
                                    }
                                }
                                else
                                {
                                    cooldown = 0.0f;
                                    audioSource.time = 0.0f;
                                    audioSource.Play();
                                    combatState = CombatState.AIMING;
                                    burstFiring = true;
                                    currentShootBurstCount--;
                                }
                            }
                            else
                            {
                                cooldown = 0.0f;
                                audioSource.time = 0.0f;
                                audioSource.Play();
                                combatState = CombatState.AIMING;                               
                            }
                        }
                        else if (!PlayerinLOS())
                        {
                            enemyState = EnemyState.IDLE;
                        }
                        
                        break;
                }
  
                break;
            
            case EnemyState.DEAD:

                if (hitTimer < 0.0f && !deathTriggered)
                {
                    deathTriggered = true;
                    Die();
                }
                else
                {
                    hitTimer -= Time.deltaTime;
                }

                break;
            
            default:

                break;
        }
    }

    private void LookAtTarget(GameObject target)
    {
        GameObject trackingPoint = lookAtTarget;

        trackingPoint.transform.position = target.transform.position;

        Vector3 targetpos = new Vector3(trackingPoint.transform.position.x, 0, trackingPoint.transform.position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        float angle = Vector3.Angle(target.transform.position - transform.position, transform.forward);
        if (angle > 30)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetpos - pos), Time.deltaTime * 5.0f);
        }
    }

    private void FireProjectile(Vector3 target)
    {
        GameObject projectile = Instantiate(projectileClass, gunPosition.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().initializeProjectile(target, projectileSpeed, projectileLifetime, false, null);
        
        // Track projectiles so we can destroy them on reset
        projectiles.Add(projectile);

        anim.Play("Shoot");

        if (audioSource)
        {
            // Lowers volume based on how close the player is
            float distance = Vector3.Distance(transform.position, player.transform.position) / detectionRadius;
            float volume = Mathf.Lerp(0.1f, 1.0f, distance);

            audioSource.PlayOneShot(shootSound, volume);
            
        }
    }

    private Vector3 AimAhead()
    {
        float t = CollisionTime();
        if(t > 0f)
        {
            Vector3 delta = t * playerRb.velocity;
            //return player.transform.position + t * playerRb.velocity;

            //try clamping up/down movement?
            Vector3 deltaNew = new Vector3(delta.x, delta.y * VertAimMod(t), delta.z);
            return player.transform.position + deltaNew;
            //Consider boosting the horizontal delta when travel time is long
        }
        else
        {
            return Vector3.up; //If this happens, something has gone wrong.
        }
    }

    private float CollisionTime() //Based on player's current velocity, returns the time of the first possible collision between bullet and player. Returns a negative number if there is no possible way for a bullet to catch up to the player.
    {
        //using instructions from https://howlingmoonsoftware.com/leading-a-target/
        Vector3 playerVel = playerRb.velocity; //In the instructions this is the relative v of the gun and the target, but I am assuming the dude isn't moving while shooting
        float bulletVel = projectileSpeed;
        Vector3 delta = player.transform.position - gunPosition.transform.position; //relative position of gun and the target

        //quadratic equation time
        float a = Vector3.Dot(playerVel, playerVel) - bulletVel * bulletVel;
        float b = 2f * Vector3.Dot(playerVel, delta);
        float c = Vector3.Dot(delta, delta);
        float det = b * b - 4f * a * c;


        if(det > 0f)
        {
            return 2f * c / (Mathf.Sqrt(det) - b);
        }
        else
        {
            return -1f;
        }
    }

    private float VertAimMod(float time)
    {
        float normTime = time / graphMax;
        return verticalAimMod.Evaluate(normTime);

    }

    public void SetHurtType(int type)
    {
        anim.SetFloat("HurtType", type);
    }

    public override void TakeDamage(int damage, Vector3 direction, float force)
    {
        anim.SetTrigger("Hit");

        if (enemyState == EnemyState.DEAD)
        {
            return;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            audioSource.Stop();
            gunLight.GetComponent<Light>().intensity = 0.0f;

            deathTriggered = false;
            hitTimer = 0.075f;
            enemyState = EnemyState.DEAD;
        }
    }

    protected override void Die()
    {
        hitbox.enabled = false;
        base.Die();
    }

    public override void ResetEnemy()
    {
        hitbox.enabled = true;
        DestroyProjectiles();
        base.ResetEnemy();
    }

    public void DestroyProjectiles()
    {
        foreach(GameObject p in projectiles)
        {
            Destroy(p);
        }
    }
}
