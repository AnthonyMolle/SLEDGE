using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public bool activated = false;
    public FMODUnity.StudioEventEmitter sfxEmitter;
    
    PlayerController playerController;
    // Saved level status (enemies killed, power-ups held)
    PlayerController.Powerup savedPowerup = PlayerController.Powerup.None;
    private Dictionary<GameObject, bool> savedEnemyStatus;
    
    // Saved score
    int savedKills;
    int savedStyle;

    private void Start()
    {
        sfxEmitter.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only activate a checkpoint if it isn't already active
        if (other.gameObject.tag == "Player" && !activated)
        {
            playerController = other.gameObject.GetComponent<PlayerController>();
            ActivateCheckpoint();
        }
    }

    // Checkpoint Activation: Resets the player's health and spawn location, and saves the current state of the level, including enemies alive, power-up equipped, and kill/style score
    private void ActivateCheckpoint()
    {
        playerController.UpdateSpawn(this);
        playerController.ResetHealth();

        savedPowerup = playerController.GetCurrentPowerup();
        savedKills = ScoreManager.Instance.GetEnemiesKilled();
        savedStyle = ScoreManager.Instance.GetStyleKills();

        activated = true;
        AudioManager.Instance.PlayOneShotSFX2D(AudioManager.Instance.CheckpointActivate);

        // Grab the current status of all enemies in the level from the level manager (dead or alive)
        if (EnemyManager.Instance != null)
        {
            savedEnemyStatus = new Dictionary<GameObject, bool>();
            foreach (KeyValuePair<GameObject, bool> entry in EnemyManager.Instance.GetEnemyStatus())
            {
                savedEnemyStatus.Add(entry.Key, entry.Value);
            }
        }

        // Visual update to indicate the checkpoint has been used
        gameObject.transform.GetChild(1).transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        sfxEmitter.Stop();
    }

    // Allow checkpoint to be used again and update visuals accordingly
    public void DeactivateCheckpoint()
    {
        activated = false;
        gameObject.transform.GetChild(1).transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
    }

    // Reset the level based on state saved when the checkpoint was activated
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
