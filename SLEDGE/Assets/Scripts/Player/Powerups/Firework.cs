using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    public int damage = 1;
    public float impactForce;
    public float lifeTime;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        if (hitObject.gameObject.CompareTag("Enemy Flyer"))
        {
            hitObject.GetComponent<FlyingEnemy>().TakeDamage(damage, Vector3.up, impactForce);
        }
        else if (hitObject.CompareTag("Enemy Shooter"))
        {
            hitObject.GetComponent<ShooterEnemy>().TakeDamage(damage, Vector3.up, impactForce);
        }
        Destroy(gameObject);
    }
}
