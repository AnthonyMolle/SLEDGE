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
    public bool canShoot = true;
    
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
    private Material baseMaterial;
    public List<GameObject> parts;      // all parts with rigidbodies/materials that can be manipulated
    public Material hitGlow;
    private MultiAimConstraint chestConstraint;
    [SerializeField] GameObject ShootSound;
    public Quaternion ogrotation;
    [SerializeField] ParticleSystem HitFX;
    [SerializeField] GameObject DeathFX;

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
        ogrotation = transform.localRotation;

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
                    if (canShoot) {
                        FireProjectile();
                    }
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
        FlashWhite(damage, direction * force);

        // rb.AddForce(direction * force, ForceMode.Impulse);
        rb.velocity = rb.velocity + direction * force;
        StartCoroutine(Pause(0.18f));
        StartCoroutine(Shake(0.2f));
        PlayParticles();
        if (direction.x <= 0) {
            StartCoroutine(Hit("left"));
        }
        else if (direction.x > 0) {
            StartCoroutine(Hit("right"));
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log("calling ded");
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
        var dfx = Instantiate(DeathFX, transform.position, transform.rotation);
        dfx.SetActive(true);
        Debug.Log($"Ragdoll: {ragdoll.name}");
        // foreach (var bone in yeesus.)  // need a way to get the position of the premortum state
        
        DestroyProjectiles();
        EnemyManager.Instance.EnemyDeath(gameObject);
        Debug.Log("ded");
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

    public IEnumerator Pause(float time)
    {
        var currentVelo = rb.velocity;
        var currentAngularVelo = rb.angularVelocity;
        anim.speed = 0;
        rb.isKinematic = true;
        yield return new WaitForSeconds(time);
        // rb.angularVelocity = currentAngularVelo;
        rb.isKinematic = false;
        rb.velocity = currentVelo;
        anim.speed = 1;
    }

    public void FlashWhite(int damage, Vector3 force)
    {
        foreach (GameObject part in parts)
        {
            part.GetComponent<Part>().TakeDamage(damage, force);
        }
    }

    public IEnumerator Shake(float time)
    {
        var timeBetweenShake = 0.02f;
        var shakeTimer = 0f;
        var power = 10f;
        var currentPower = 0f;
        var scale = 0.5f;
        var currentScale = 0f;
        var shakeTime = time;
        float xRot = 0f;
        float yRot = 0f;
        float zRot = 0f;
        var ogScale = transform.localScale;
        while (shakeTimer < shakeTime)
        {
            shakeTimer += timeBetweenShake;
            currentPower = power * (shakeTime - shakeTimer)/ shakeTime;
            currentScale = 1f + scale * (shakeTime - shakeTimer)/ shakeTime;
            xRot = Random.Range(-currentPower, currentPower);
            yRot = Random.Range(-currentPower, currentPower);
            transform.Rotate(-xRot, -yRot, -zRot, Space.Self);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            // zRot = Random.Range(-currentPower, currentPower);

            transform.Rotate(xRot, yRot, 0, Space.Self);
            //hammerCameraObject.transform.localPosition = new Vector3(-xPos, -yPos, originalHammerCamPos.z);


            yield return new WaitForSeconds(timeBetweenShake);
        }
        Debug.Log("HUZZAH");
        transform.localScale = new Vector3(1, 1, 1);
        transform.localRotation = ogrotation;
    }

    public IEnumerator Hit(string dir)
    {
        Debug.Log("Hit coroutine run");
        if (dir == "left")
        {
            anim.Play("shooter_hit_left 0");
            // anim.speed = 0f;
        }
        if (dir == "right")
        {
            anim.Play("shooter_hit_right 0");
            // anim.speed = 0f;
        }
        // yield return new WaitForSeconds(0.05f);
        anim.speed = 1f;
        yield return null;
    }

    public void PlayParticles()
    {
        HitFX.Play();
    }
}
