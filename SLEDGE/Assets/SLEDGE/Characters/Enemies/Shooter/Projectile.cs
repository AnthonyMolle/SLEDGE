using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Basic stats
    float bulletSpeed = 5.0f;
    float maxLifetime = 10.0f;
    float lifetime = 0.0f;
    
    // Projectile target tracking
    bool isParried = false;
    GameObject target;
    [SerializeField] float trackingRotationSpeed = 5f;
    [SerializeField] float maximumRotation = 30f;
    float currentRotation = 0f;

    // For using different hitboxes for normal vs parried shots
    [SerializeField] Collider parriedCollider;
    [SerializeField] Collider normalCollider;

    // Update is called once per frame
    void Update()
    {
        // Basic movement + lifespan tracking
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        lifetime += Time.deltaTime;

        // Parry tracking: Parried shots can target and rotate towards enemies to an extent, making parrying easier, so we have to calculate their rotation
        if (isParried)
        {
            if (target != null && currentRotation < maximumRotation)
            {
                Vector3 rotation = Vector3.RotateTowards(transform.forward, target.transform.position - transform.position, trackingRotationSpeed * Time.deltaTime, 0 * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(rotation);

                currentRotation += trackingRotationSpeed * Time.deltaTime;
            }
        }
        else
        {
            currentRotation = 0f;
        }

        // Once a projectile reaches its lifespan, we delete it
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    // Initializes projectile stats with inputted values, so we can use the same projectile class for functionally different projectiles (such as parried shots)
    public void initializeProjectile(Vector3 direction, float speed, float life, bool parried, GameObject sentTarget)
    {
        currentRotation = 0f;

        transform.LookAt(direction);
        bulletSpeed = speed;
        maxLifetime = life;
        isParried = parried;

        if (isParried)
        {
            parriedCollider.enabled = true;
            normalCollider.enabled = false;
        }

        target = sentTarget;

        return;
    }

    // Collision + Damage Handling
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Don't damage the player with parried projectiles (since they own the projectile at that point)
            if (!isParried)
            {
                Destroy(gameObject);
                other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
            }
            return;
        }
        if (other.gameObject.transform.root.CompareTag("Enemy Flyer") || other.gameObject.transform.root.CompareTag("Enemy Shooter"))
        {
            GameObject enemy = other.gameObject.transform.root.gameObject;
            // Only damage enemies if this projectile was parried and the enemy isn't already dead
            if (isParried && enemy.GetComponent<EnemyBaseController>().GetHealth() > 0)
            {
                Destroy(gameObject);
                enemy.GetComponent<EnemyBaseController>().TakeDamage(1, new Vector3(0, 0, -1), 10f);
                GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddStyleKills(400); // Parry kills get style points!
            }
            return;
        }
        Destroy(gameObject);
    }

    public bool CheckParried()
    {
        return isParried;
    }
}
