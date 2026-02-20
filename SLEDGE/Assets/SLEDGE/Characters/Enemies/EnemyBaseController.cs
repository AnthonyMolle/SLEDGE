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

    [SerializeField] RagdollScript deathRagdoll;

    [Tooltip("How long the ragdoll will remain before disappearing (set to -1 to stay forever)")]
    public float decayTime = 5.0f;
    float decayTimer = -1f;
    bool fadeTriggered = false;
    FadeOut fadeScript;

    protected Vector3 spawnPosition;
    protected Rigidbody rb;

    protected GameObject player;
    protected Rigidbody playerRb;

    RaycastHit[] hits;
    public LayerMask playerMask;

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
        playerRb = player.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();

        spawnPosition = transform.position;
        currentHealth = maxHealth;

        fadeScript = GetComponent<FadeOut>();
    }

    protected virtual void Update()
    {
        if (decayTimer > 0)
        {
            decayTimer -= Time.deltaTime;
            if (decayTimer < 1 && !fadeTriggered)
            {
                fadeTriggered = true;
                fadeScript.Fade();
            }
        }
        else if (decayTimer != -1)
        {
            deathRagdoll.KillSwitch(false);
            deathRagdoll.gameObject.SetActive(false);
            decayTimer = -1;
        }
    }

    protected bool PlayerinLOS()
    {
        hits = Physics.RaycastAll(transform.position, player.transform.position - transform.position, detectionRadius, playerMask, QueryTriggerInteraction.Ignore);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public virtual void TakeDamage(int damage, Vector3 direction, float force)
    {
        if (enemyState == EnemyState.DEAD)
        {
            return;
        }

        //rb.AddForce(direction * force, ForceMode.Impulse);
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
            decayTimer = decayTime;
            deathRagdoll.KillSwitch(true);
            EnemyManager.Instance.EnemyDeath(gameObject);
            return;
        }

        // Enemy manager marks this enemy as dead
        EnemyManager.Instance.EnemyDeath(gameObject);
        gameObject.SetActive(false);
    }

    public virtual void ResetEnemy()
    {
        if (deathRagdoll)
        {
            deathRagdoll.gameObject.SetActive(true);
            deathRagdoll.KillSwitch(false);
            decayTimer = -1;
            fadeTriggered = false;
            fadeScript.ResetMaterials();
        }
        transform.position = spawnPosition;
        enemyState = EnemyState.IDLE;
    }
}
