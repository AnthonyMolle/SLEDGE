using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] FlyingEnemy[] flyingEnemies;
    [SerializeField] int spawnPointIndex;

    bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !activated)
        {
            other.gameObject.GetComponent<PlayerController>().UpdateSpawn(spawnPointIndex, this);
            activated = true;
        }
    }

    public void Reset()
    {
        foreach (FlyingEnemy enemy in flyingEnemies)
        {
            enemy.Reset();
        }
    }
}
