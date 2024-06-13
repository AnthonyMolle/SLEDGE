using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Spawner[] enemies;
    [SerializeField] int spawnPointIndex;

    public bool activated = false;
    private Dictionary<GameObject, bool> savedEnemyStatus;

    private void Start()
    {
        //savedEnemyStatus = EnemyManager.Instance.GetEnemyStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !activated)
        {
            other.gameObject.GetComponent<PlayerController>().UpdateSpawn(this);
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        activated = true;
        //savedEnemyStatus = EnemyManager.Instance.GetEnemyStatus();
        savedEnemyStatus = new Dictionary<GameObject, bool>();
        foreach (KeyValuePair<GameObject, bool> entry in EnemyManager.Instance.GetEnemyStatus())
        {
            savedEnemyStatus.Add(entry.Key, entry.Value);
        }

        gameObject.transform.GetChild(1).transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
    }

    public void DeactivateCheckpoint()
    {
        activated = false;
        gameObject.transform.GetChild(1).transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
    }

    public void ResetState()
    {
        EnemyManager.Instance.ResetEnemyState(savedEnemyStatus);
    }
}
