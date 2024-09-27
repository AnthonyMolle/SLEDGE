using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private Dictionary<GameObject, bool> enemyStatus = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject, bool> GetEnemyStatus() { return enemyStatus; }
    void Start()
    {

        GameObject[] flyerEnemies = GameObject.FindGameObjectsWithTag("Enemy Flyer");
        GameObject[] shooterEnemies = GameObject.FindGameObjectsWithTag("Enemy Shooter");
        List<GameObject> enemies = new List<GameObject>();
        enemies.AddRange(flyerEnemies);
        enemies.AddRange(shooterEnemies);

        foreach (GameObject enemy in enemies)
        {
            enemyStatus.Add(enemy, true);
        }
    }


    public void EnemyDeath(GameObject enemy)
    {
        if (enemyStatus.ContainsKey(enemy))
        {
            enemyStatus[enemy] = false;
        }
    }

    public void ResetEnemyState(Dictionary<GameObject, bool> savedEnemyStatus)
    {
        foreach (KeyValuePair<GameObject, bool> entry in savedEnemyStatus)
        {
            GameObject enemy = entry.Key;
            bool isAlive = entry.Value;

            if (enemyStatus.ContainsKey(enemy))
            {

                // Respawn enemy if saved status is alive but current status is dead
                if (isAlive && !enemyStatus[enemy])
                {
                    RespawnEnemy(enemy);
                }
                // Also respawn any alive enemies so their position/state are reset
                else if (enemyStatus[enemy])
                {
                    RespawnEnemy(enemy);
                }

                enemyStatus[enemy] = isAlive;
            }
        }
    }

    private void RespawnEnemy(GameObject enemy)
    {
        enemy.SetActive(true);
        if (enemy.GetComponent<FlyingEnemy>() != null)
        {
            enemy.GetComponent<FlyingEnemy>().Reset();
        }
        else if (enemy.GetComponent<ShooterEnemy>() != null)
        {
            enemy.GetComponent<ShooterEnemy>().Reset();
        }
    }
}
