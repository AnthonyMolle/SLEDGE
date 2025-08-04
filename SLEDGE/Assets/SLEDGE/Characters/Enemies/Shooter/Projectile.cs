using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float bulletSpeed = 5.0f;
    float maxLifetime = 10.0f;
    float lifetime = 0.0f;
    

    bool isParried = false;
    GameObject target;
    [SerializeField] float trackingRotationSpeed = 5f;
    [SerializeField] float maximumRotation = 30f;
    float currentRotation = 0f;

    [SerializeField] Collider parriedCollider;
    [SerializeField] Collider normalCollider;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        lifetime += Time.deltaTime;

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

        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }

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

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("blah");
        if (other.gameObject.tag == "Player")
        {
            if (!isParried)
            {
                Destroy(gameObject);
                other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
            }
        }
        else if (other.gameObject.tag == "Enemy Flyer" && isParried && other.gameObject.GetComponent<EnemyStatsController>().GetHealth() > 0)
        {
            Destroy(gameObject);
            //FindObjectOfType<Hitstop>().Stop(hitStopDuration);
            other.gameObject.GetComponent<EnemyStatsController>().TakeDamage(1, new Vector3(0, 0, -1), 10f);
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddStyleKills(400);
        }
        else if (other.gameObject.tag == "Enemy Shooter" && isParried && other.gameObject.GetComponent<EnemyShooterController>().GetHealth() > 0)
        {
            Destroy(gameObject);
            //FindObjectOfType<Hitstop>().Stop(hitStopDuration);
            other.gameObject.GetComponent<EnemyShooterController>().TakeDamage(1, new Vector3(0, 0, -1), 10f);
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddStyleKills(400);
        }
        else if (other.gameObject.tag == "Enemy Shooter")
        {

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CheckParried()
    {
        return isParried;
    }
}
