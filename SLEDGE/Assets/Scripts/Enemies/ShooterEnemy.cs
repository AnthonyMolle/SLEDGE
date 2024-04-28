using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Vector3 position;

    GameObject player;
    float cooldown = 2.0f;
    RaycastHit hit;

    Transform gun;
    GameObject projectile;
    List<GameObject> projectiles = new List<GameObject>();

    public enum EnemyState
    {
        IDLE,
        HOSTILE
    }

    EnemyState enemyState = EnemyState.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

        gun = transform.Find("Gun");
        position = transform.position;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        rb.velocity = Vector3.zero;
        transform.position = position;
        cooldown += Time.deltaTime;
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

            default:
                break;
        }
    }

    private bool PlayerinLOS()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionRadius))
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
        projectile.GetComponent<Projectile>().initializeProjectile(player.transform.position, bulletSpeed, bulletLifetime, false);
        return;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // add sfx and vfx and such!
        GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddEnemiesKilled(1);
        Destroy(gameObject);
    }

    public void Destroy()
    {
        foreach (GameObject p in projectiles)
        {
            Destroy(p);
        }
        Destroy(gameObject);
    }

}
