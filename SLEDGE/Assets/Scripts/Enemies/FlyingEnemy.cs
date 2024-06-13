using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// Jonah Ryan

public class FlyingEnemy : MonoBehaviour
{

    [Header("Combat")]
    [SerializeField] int maxHealth = 1;
    int currentHealth = 1;

    [Header("Movement Properties")]
    public float movementSpeed = 5;
    

    [Header("Vision Properties")]
    public float detectionRadius = 5;
    public LayerMask visionBlocking;

    [Header("Debug")]
    public Color huntingColor;
    public Color pathingColor;


    Rigidbody rb;
    Vector3 targetPos;
    Vector3 currentWaypoint;
    Transform player;
    int currentIndexOnPath = -1;
    RaycastHit hit;

    public enum EnemyState
    {
        IDLE,
        DIRECTHUNT,
        ONPATH,
        STUNNED
    }

    [Header("Death")]
    public GameObject deathRagdoll;

    EnemyState enemyState = EnemyState.IDLE;

    // Setup our dependencies
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }
    
    private void Update()
    {
        transform.LookAt(player);
    }

    // Check if a given transform at endPoint is visible from a startPoint
    // Visible means a raycast can be drawn between the two points without hitting a collider
    private bool transformInView(Vector3 startPoint, Vector3 endPoint, Transform targetTransform, float distanceOfVision)
    {
        Vector3 EnemytoPlayer = endPoint - startPoint;

        if (Physics.Raycast(startPoint, EnemytoPlayer, out hit, distanceOfVision, visionBlocking))
        {
            if (hit.transform == targetTransform)
            {
                return true;
            }
        }

        return false;
    }

    // Process our current state
    private void FixedUpdate()
    {

        rb.velocity = Vector3.zero;

        switch (enemyState)
        {
            /* 
             * Idle state:
             * - Waits for player to be in range
             * - Then starts hunting player
            */
            case EnemyState.IDLE:

                // Aggro when player 10 units away
                if (transformInView(transform.position, player.position, player, detectionRadius))
                {
                    enemyState = EnemyState.DIRECTHUNT;
                }
                break;

            /*
             * Hunt State:
             * - When player is visible go straight towards them
             * - Otherwise, switch to pathing mode
             */
            case EnemyState.DIRECTHUNT:

                //GetComponent<Renderer>().material.color = huntingColor;

                // Move towards player
                targetPos = Vector3.MoveTowards(transform.position, player.position, movementSpeed * Time.deltaTime);
                rb.MovePosition(targetPos);

                // If player not in line of sight, pathfind to player
                if (!transformInView(transform.position, player.position, player, Mathf.Infinity))
                {
                    enemyState = EnemyState.ONPATH;
                }

                break;

            /*
             * Path state:
             *  - When player not visible, follow known path
             *  - When player visible, go back to hunt state
             */
            case EnemyState.ONPATH:

                //GetComponent<Renderer>().material.color = pathingColor;

                // Initialize path following with closest node on path
                if (currentIndexOnPath == -1)
                {
                    currentIndexOnPath = PlayerTracker.getNearestOnPath(transform.position);
                    currentWaypoint = PlayerTracker.getIndexOnPath(currentIndexOnPath);
                }

                // If you can see player, leave pathing, enter hunt state
                if (transformInView(transform.position, player.position, player, Mathf.Infinity))
                {
                    enemyState = EnemyState.DIRECTHUNT;
                    currentIndexOnPath = -1;
                    break;
                }

                // If reached current path target, go forward on path
                if(Vector3.Distance(transform.position, currentWaypoint) < 1)
                {
                    currentIndexOnPath += 1;

                    // If reached end of path, restart path following with new path information
                    if(PlayerTracker.getSize() <= currentIndexOnPath)
                    {
                        currentIndexOnPath = PlayerTracker.getNearestOnPath(transform.position);
                        currentWaypoint = PlayerTracker.getIndexOnPath(currentIndexOnPath);
                        break;
                    }

                    // Update target to next on path
                    currentWaypoint = PlayerTracker.getIndexOnPath(currentIndexOnPath);
                }

                // Move towards our current path target
                targetPos = Vector3.MoveTowards(transform.position, currentWaypoint, movementSpeed * Time.deltaTime);
                rb.MovePosition(targetPos);

                break;
            
            case EnemyState.STUNNED:
                break;

        }
    }

    Vector3 startPosition;

    public void Reset()
    {
        transform.position = startPosition;
        enemyState = EnemyState.IDLE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
            TakeDamage(1, new Vector3(0, 0, -1), 10f);
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage, Vector3 direction, float force)
    {
        enemyState = EnemyState.STUNNED;
        rb.AddForce(direction * force, ForceMode.Impulse);
        Debug.Log("afterlocity: "+ rb.velocity);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die(direction * force);
        }
    }

    // public void Knockback(Vector3 direction, float force)
    // {
    //     Debug.Log("hello");
    //     Vector3 dir = new Vector3(-0.5f, 0.5f, 0.5f);
    //     float frace = 15f;
    //     rb.AddForce(dir * frace, ForceMode.Impulse);
    //     Debug.Log(rb.velocity);
    // }


    public void Die(Vector3 force)
    {
        Debug.Log("DIEEEEEEE");
        // add sfx and vfx and such!
        GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddEnemiesKilled(1);

        GameObject cheesus = Instantiate(deathRagdoll, transform.position, transform.rotation);
        cheesus.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        EnemyManager.Instance.EnemyDeath(gameObject);
        gameObject.SetActive(false);
    }
}
