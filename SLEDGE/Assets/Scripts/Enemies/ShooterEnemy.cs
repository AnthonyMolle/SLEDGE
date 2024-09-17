using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ShooterEnemy : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] int maxHealth = 1;
    int currentHealth = 1;
    public GameObject projectileType;
    
    [SerializeField] float bulletLifetime = 5.0f;
    [SerializeField] float bulletSpeed = 0.2f;

    [Header("Movement Properties")]
    public float movementSpeed = 5;


    [Header("Vision Properties")]
    public float detectionRadius = 20;

    Rigidbody rb;
    Vector3 startPosition;

    GameObject player;
    float cooldown = 2.0f;
    RaycastHit hit;

    public Transform gun;
    GameObject projectile;
    List<GameObject> projectiles = new List<GameObject>();

    [Header("Animation")]
    public GameObject deathRagdoll;
    public GameObject rig;
    public Animator anim;
    public GameObject lookAtTarget;
    public List<GameObject> trackConstraints;
    public float angle;
    private MultiAimConstraint chestConstraint;
    [SerializeField] GameObject ShootSound;

    GameObject ragdoll;

    public enum EnemyState
    {
        IDLE,
        HOSTILE,
        STUNNED
    }

    EnemyState enemyState = EnemyState.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

        // gun = transform.Find("Gun");
        startPosition = transform.position;
        currentHealth = maxHealth;
        chestConstraint = trackConstraints[0].GetComponent<MultiAimConstraint>();
    }

    // Update is called once per frame
    void Update()
    {
        lookAtTarget.transform.position = player.transform.position;
        // transform.rotation = Quaternion.LookRotation(lookPos);
        rb.velocity = Vector3.zero;
        cooldown += Time.deltaTime;
        // rotate dude if you are behind him
        Vector3 targetpos = new Vector3(lookAtTarget.transform.position.x, 0, lookAtTarget.transform.position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        angle = Vector3.Angle(lookAtTarget.transform.position - transform.position, transform.forward);
        if (angle > 130) {
            transform.rotation = Quaternion.LookRotation(targetpos - pos);
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
                if (PlayerinLOS() && cooldown >= 2.0f)
                {
                    cooldown = 0.0f;
                    FireProjectile();
                } else if (!PlayerinLOS())
                {
                    enemyState = EnemyState.IDLE;
                }
                break;

            case EnemyState.STUNNED:
                break;

            default:
                break;
        }
    }

    private bool PlayerinLOS()
    {
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, detectionRadius))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    private void FireProjectile()
    {
        // Debug.Log("Firing");
        // Debug.Log(player.transform.position);
        projectile = Instantiate(projectileType, gun.position, Quaternion.identity);
        projectiles.Add(projectile);
        projectile.GetComponent<Projectile>().initializeProjectile(player.transform.position, bulletSpeed, bulletLifetime, false, null);
        Instantiate(ShootSound, gameObject.transform.position, Quaternion.identity);
        return;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage, Vector3 direction, float force)
    {
        Debug.Log("danage");
        enemyState = EnemyState.STUNNED;
        rb.AddForce(direction * force, ForceMode.Impulse);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // public void Knockback(Vector3 direction, float force)
    // {
    //     Debug.Log("hello");
    //     Vector3 dir = new Vector3(-0.5f, 0.5f, 0.5f);
    //     float frace = 15f;
    //     rb.AddForce(dir * frace, ForceMode.Impulse);
    //     Debug.Log("you just been forced");
    //     Debug.Log(rb.velocity);
    // }

    private void Die()
    {
        // add sfx and vfx and such!
        GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddEnemiesKilled(1);
        ragdoll = Instantiate(deathRagdoll, transform.position, transform.rotation);
        ragdoll.GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.Impulse);
        // foreach (var bone in yeesus.)  // need a way to get the position of the premortum state
        
        DestroyProjectiles();
        EnemyManager.Instance.EnemyDeath(gameObject);
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        DestroyProjectiles();
        Destroy(ragdoll);
        ragdoll = null;
        transform.position = startPosition;
        enemyState = EnemyState.IDLE;
    }

    public void DestroyProjectiles()
    {
        foreach (GameObject p in projectiles)
        {
            Destroy(p);
        }
    }

}
