using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Spawner[] enemies;
    [SerializeField] int spawnPointIndex;

    bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !activated)
        {
            other.gameObject.GetComponent<PlayerController>().UpdateSpawn(spawnPointIndex, this);
            activated = true;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void Reset()
    {
        foreach (Spawner spawner in enemies)
        {
            spawner.Respawn();
        }
    }
}
