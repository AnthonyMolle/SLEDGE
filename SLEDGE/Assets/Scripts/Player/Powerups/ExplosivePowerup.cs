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
        powerupManager.AddPowerup(GetName());
        playerController.onHammerBounce.AddListener(UsePowerup);
        playerController.IncreaseInitialForce(forceAddend);
    }

    public void UsePowerup()
    {
        if (powerupManager.IsCurrentPowerup(GetName()))
        {
            playerController.IncreaseInitialForce(-forceAddend);
            playerController.onHammerBounce.RemoveListener(UsePowerup);
            //powerupManager.RemoveCurrentPowerup();
            powerupManager.UsePowerup();
            Destroy(this);
        }
    }

    public string GetName()
    {
        return GetType().ToString();
    }
}
