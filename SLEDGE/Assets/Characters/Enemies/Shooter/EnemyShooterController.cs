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

    // Projectile type
    [SerializeField] GameObject projectileClass;
    
    // Handling projectile instances
    GameObject projectile;
    List<GameObject> projectiles = new List<GameObject>();
    
    // Cooldown between shots
    [SerializeField] float shootCooldown = 3.0f;
    float cooldown;

    // Projectile stats
    [SerializeField] float projectileLifetime;
    [SerializeField] float projectileSpeed;

    [Header("Positional Constants")]
    [HorizontalLine]

    // Used for rotating to face player
    [SerializeField] GameObject lookAtTarget;

    // Where to fire projectiles from
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
    float aimTimer = 0.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
        // Enemy Behaviors
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
                if (combatState == CombatState.AIMING && aimTimer >= 2.0f)
                {
                    combatState = CombatState.COOLDOWN;
                    aimTimer = 0.0f;
                    
                    audioSource.Stop();
                    gunLight.GetComponent<Light>().intensity = 0.0f;
                    
                    FireProjectile(player.transform.position);
                }
                
                if (PlayerinLOS() && cooldown >= shootCooldown)
                {
                    cooldown = 0.0f;
                    audioSource.time = 0.0f;
                    audioSource.Play();
                    combatState = CombatState.AIMING;
                } 
                else if (!PlayerinLOS())
                {
                    enemyState = EnemyState.IDLE;
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
