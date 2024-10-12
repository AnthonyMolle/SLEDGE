using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsController : MonoBehaviour
{
    // ONLY PUT BEHAVIOURS THAT ARE REACTIONS
    // ALL PATTERNED BEHAVIOURS MUST BE IN BEHAVIOUR TREE
    // OR I WILL COME FOR YOU - Jonah <3


    [Header("Child references on enemy")]
    [HorizontalLine]

    public GameObject bulletSource;
    public GameObject lookAtTarget;

    [Header("Medical Information")]
    [HorizontalLine]
    [SerializeField] int maxHealth = 1;
    int currentHealth = 1;

    [Header("Animation")]
    [HorizontalLine]
    public GameObject deathRagdoll;

    // Private trackers
    Blackboard Blackboard;
    Rigidbody rb;
    GameObject ragdollInstance;
    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        Blackboard = GetComponent<BehaviorTreeRunner>().tree.blackboard;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage, Vector3 direction, float force)
    {
        Blackboard.currentState = Blackboard.EnemyStates.STUNNED;
        rb.AddForce(direction * force, ForceMode.Impulse);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddEnemiesKilled(1);

        ragdollInstance = Instantiate(deathRagdoll, transform.position, transform.rotation);
        ragdollInstance.GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.Impulse);

        //DestroyProjectiles();
        EnemyManager.Instance.EnemyDeath(gameObject);
        gameObject.SetActive(false);
    }
    public void ResetEnemy()
    {
        //DestroyProjectiles();
        Destroy(ragdollInstance);
        ragdollInstance = null;
        transform.position = startPosition;
        if (Blackboard != null)
        {
            Blackboard.currentState = Blackboard.EnemyStates.IDLE;
        }
    }

}
