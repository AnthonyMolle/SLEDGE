using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksPowerup : MonoBehaviour
{
    PlayerController playerController;
    PowerupManager powerupManager;

    //This value has to be manually changed in code!
    public int numFireworks = 3;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        powerupManager = GetComponent<PowerupManager>();
        powerupManager.AddPowerup(GetType().ToString());
        playerController.onHammerSwipe.AddListener(LaunchFireworks);
    }

    public void LaunchFireworks()
    {
        playerController.SpawnFireworks();
        numFireworks--;

        if (numFireworks <= 0 )
        {
            RemovePowerup();
        }
    }

    public void RemovePowerup()
    {
        playerController.onHammerSwipe.RemoveListener(RemovePowerup);
        powerupManager.RemoveCurrentPowerup();
        Destroy(this);
    }
}
