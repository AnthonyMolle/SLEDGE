using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float bulletSpeed = 5.0f;
    float maxLifetime = 10.0f;
    float lifetime = 0.0f;
    public float hitStopDuration;
    public GameObject sentEnemy;

    bool isParried = false;

    public Collider parriedCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        lifetime += Time.deltaTime;

        if (isParried)
        {
            if (sentEnemy != null)
            {
                
                Vector3 rotation = Vector3.RotateTowards(transform.forward, sentEnemy.transform.position - transform.position, 5 * Time.deltaTime, 0 * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(rotation);

                Debug.Log(transform.forward + ", " + rotation);
            }
        }

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

        if (isParried)
        {
            parriedCollider.enabled = true;
        }

        return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isParried)
            {
                Destroy(gameObject);
                other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
            }
        }
        else if (other.gameObject.tag == "Enemy Flyer" && isParried && other.gameObject.GetComponent<FlyingEnemy>().GetHealth() > 0)
        {
            Destroy(gameObject);
            //FindObjectOfType<Hitstop>().Stop(hitStopDuration);
            other.gameObject.GetComponent<FlyingEnemy>().TakeDamage(1);
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddStyleKills(1);
        }
        else if (other.gameObject.tag == "Enemy Shooter" && isParried && other.gameObject.GetComponent<ShooterEnemy>().GetHealth() > 0)
        {
            Destroy(gameObject);
            //FindObjectOfType<Hitstop>().Stop(hitStopDuration);
            other.gameObject.GetComponent<ShooterEnemy>().TakeDamage(1);
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddStyleKills(1);
        }
        else if (other.gameObject.tag == "Enemy Shooter")
        {

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
