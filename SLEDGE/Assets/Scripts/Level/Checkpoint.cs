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
    
    PlayerController playerController;
    PlayerController.Powerup savedPowerup = PlayerController.Powerup.None;
    int savedKills;
    int savedStyle;

    private void Start()
    {
        //savedEnemyStatus = EnemyManager.Instance.GetEnemyStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !activated)
        {
            playerController = other.gameObject.GetComponent<PlayerController>();
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        playerController.UpdateSpawn(this);
        playerController.ResetHealth();
        
        savedPowerup = playerController.GetCurrentPowerup();
        savedKills = ScoreManager.Instance.GetEnemiesKilled();
        savedStyle = ScoreManager.Instance.GetStyleKills();
        
        activated = true;

        if (EnemyManager.Instance != null)
        {
            savedEnemyStatus = new Dictionary<GameObject, bool>();
            foreach (KeyValuePair<GameObject, bool> entry in EnemyManager.Instance.GetEnemyStatus())
            {
                savedEnemyStatus.Add(entry.Key, entry.Value);
            }
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
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.ResetEnemyState(savedEnemyStatus);
        }
        if (savedPowerup != PlayerController.Powerup.None)
        {
            playerController.CollectPowerup(savedPowerup);
        }
        ScoreManager.Instance.ResetKills(savedKills);
        ScoreManager.Instance.ResetStyle(savedStyle);
    }
}
