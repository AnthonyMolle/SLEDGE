using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SeismicPowerup : MonoBehaviour
{
    PlayerController playerController;
    PowerupManager powerupManager;

    int quakeDamage = 1;
    float quakeRadius = 5f;
    float quakeForce = 1f;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        powerupManager = GetComponent<PowerupManager>();
        powerupManager.AddPowerup(GetType().ToString());
        playerController.onHammerBounce.AddListener(Quake);
        playerController.onHammerBounce.AddListener(RemovePowerup);
    }

    void Quake()
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, quakeRadius);

        foreach (Collider hitObject in hitObjects)
        {
            if (hitObject.CompareTag("Enemy Flyer"))
            {
                hitObject.GetComponent<EnemyStatsController>().TakeDamage(quakeDamage, Vector3.up, quakeForce);
            }
            else if (hitObject.CompareTag("Enemy Shooter"))
            {
                hitObject.GetComponent<EnemyStatsController>().TakeDamage(quakeDamage, Vector3.up, quakeForce);
            }
        }
    }

    public void RemovePowerup()
    {
        playerController.onHammerBounce.RemoveListener(RemovePowerup);
        powerupManager.RemoveCurrentPowerup();
        Destroy(this);
    }
}
