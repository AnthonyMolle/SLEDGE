using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform spawnLocation;
    GameObject currentEnemy;

    void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        currentEnemy = Instantiate(enemyPrefab, spawnLocation);
    }

    public void Respawn()
    {
        if (currentEnemy != null)
        {
            if (currentEnemy.gameObject.name.Contains("Shooter"))
            {
                currentEnemy.gameObject.GetComponent<ShooterEnemy>().Destroy();
            }
            else
            {
                Destroy(currentEnemy);
            }
        }

        Spawn();
    }
}
