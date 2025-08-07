using Autodesk.Fbx;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour
{
    // NOTE: Any variables you want to be used by child classes must be public or protected!

    [Header("Base Stats")]
    [HorizontalLine]
    
    [SerializeField] protected int maxHealth = 1;
    protected int currentHealth = 1;
    
    [SerializeField] protected float movementSpeed;
    
    [SerializeField] protected float detectionRadius;

    [Header("Base Ragdoll")]
    [HorizontalLine]
    
    [SerializeField] GameObject deathRagdoll;
    GameObject ragdollInstance;

    protected Vector3 spawnPosition;
    protected Rigidbody rb;

    protected GameObject player;

    RaycastHit hit;

    public enum EnemyState
    {
        IDLE,
        HOSTILE,
        DEAD
    }

    protected EnemyState enemyState = EnemyState.IDLE;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

        spawnPosition = transform.position;
        currentHealth = maxHealth;
    }

    protected bool PlayerinLOS()
    {
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, detectionRadius))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage, Vector3 direction, float force)
    {
        if (enemyState == EnemyState.DEAD)
        {
            return;
        }

        rb.AddForce(direction * force, ForceMode.Impulse);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            enemyState = EnemyState.DEAD;
            Die();
        }
    }

    protected virtual void Die()
    {
        // Score tracking
        GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddEnemiesKilled(1);

        // Creating a ragdoll
        if (deathRagdoll)
        {
            ragdollInstance = Instantiate(deathRagdoll, transform.position, transform.rotation);
            ragdollInstance.GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.Impulse);
        }

        // Enemy manager marks this enemy as dead
        EnemyManager.Instance.EnemyDeath(gameObject);

        gameObject.SetActive(false);

    }

    public virtual void ResetEnemy()
    {
        if (deathRagdoll)
        {
            Destroy(ragdollInstance);
            ragdollInstance = null;
        }
        transform.position = spawnPosition;
        enemyState = EnemyState.IDLE;
    }
}
