using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePowerup : MonoBehaviour
{
    PlayerController playerController;
    PowerupManager powerupManager;

    [Tooltip("How much additional force does this powerup provide?")]
    public float forceAddend = 25f;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        powerupManager = GetComponent<PowerupManager>();
        powerupManager.AddPowerup(GetType().ToString());
        playerController.onHammerBounce.AddListener(RemovePowerup);
        playerController.IncreaseInitialForce(forceAddend);
    }

    public void RemovePowerup()
    {
        playerController.IncreaseInitialForce(-forceAddend);
        playerController.onHammerBounce.RemoveListener(RemovePowerup);
        AudioManager.Instance.PlayOneShotSFX2D(AudioManager.Instance.PowerupExplosive);

        powerupManager.RemoveCurrentPowerup();
        Destroy(this);
    }
}
