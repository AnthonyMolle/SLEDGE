using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float bulletSpeed = 5.0f;
    float maxLifetime = 10.0f;
    float lifetime = 0.0f;

    bool isParried = false;
    
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * bulletSpeed;
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    public void initializeProjectile(Vector3 direction, float speed, float life, bool parried)
    {
        transform.LookAt(direction);
        bulletSpeed = speed;
        maxLifetime = life;
        isParried = parried;
        return;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
        }
        else if (other.gameObject.tag == "Enemy Flyer" && isParried)
        {
            other.gameObject.GetComponent<FlyingEnemy>().TakeDamage(1);
        }
        else if (other.gameObject.tag == "Enemy Shooter" && isParried)
        {
            other.gameObject.GetComponent<ShooterEnemy>().TakeDamage(1);
        }
    }
}
