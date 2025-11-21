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

    [SerializeField] float aimDuration = 2.0f;
    float aimTimer = 0.0f;

    /* Projectile stats */
    [SerializeField] float projectileLifetime;
    [SerializeField] float projectileSpeed;

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
        
        currentShootBurstCount = shootBurstCount;
        cooldown = shootCooldown;
        audioSource = GetComponent<AudioSource>();

        gunLight.GetComponent<Light>().intensity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (combatState == CombatState.AIMING)
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
                        
                        if (aimTimer >= aimDuration)
                        {
                            aimTimer = 0.0f;
                            combatState = CombatState.COOLDOWN;

                            audioSource.Stop();
                            gunLight.GetComponent<Light>().intensity = 0.0f;

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
                                    }
                                }
                                else
                                {
                                    cooldown = 0.0f;
                                    audioSource.time = 0.0f;
                                    audioSource.Play();
                                    currentShootBurstCount--;
                                    combatState = CombatState.AIMING;  
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
        if (angle > 70)
        {
            transform.rotation = Quaternion.LookRotation(targetpos - pos);
        }
    }

    private void FireProjectile(Vector3 target)
    {
        GameObject projectile = Instantiate(projectileClass, gunPosition.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().initializeProjectile(target, projectileSpeed, projectileLifetime, false, null);
        
        // Track projectiles so we can destroy them on reset
        projectiles.Add(projectile);

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
            Vector3 deltaClamp = new Vector3(delta.x, delta.y * 0.5f, delta.z);
            return player.transform.position + deltaClamp;
            //TODO clamp more if time high, clamp less if time low. Consider boosting the horizontal delta. Use anim graphs for easy?
        }
        else
        {
            return Vector3.up;
        }
        //TODO: 
    }

    private float CollisionTime()
    {
        //Check that instantiation of bullet with gun position isn't going to fuck shit up with new formula
        //using instructiosn from howlingmoonsoftware.com
        Vector3 playerVel = playerRb.velocity; //may need to get rigidbody component from player first. In the instructions this is the relative v of the gun and the target, but I am assuming the dude isn't moving while shooting
        float bulletVel = projectileSpeed; //technically unnecessary yeah
        Vector3 delta = player.transform.position - gunPosition.transform.position; //relative position of gun and the target. Gun as in the bullet spawn point I believe. InverseTransformPoint is affected by scale. bad.

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

    /*private float CollisionTimeTwo()
    {
        Vector3 playerVelInitial = playerRb.velocity;
        Vector3 playerVelFinal = playerVelInitial + playerRb;
        float bulletVel = projectileSpeed;
        Vector3 playerStart = player.transform.position - gunPosition.transform.position; //relative to gun position

    }*/

    public override void ResetEnemy()
    {
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
