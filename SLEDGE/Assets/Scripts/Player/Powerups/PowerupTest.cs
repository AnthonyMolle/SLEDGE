using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupTest : MonoBehaviour
{
    PlayerController playerController;
    PowerupManager powerupManager;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        powerupManager.AddPowerup(GetType().ToString());
        playerController.onHammerBounce.AddListener(Test);
    }

    void Test()
    {
        Debug.Log("Fuck yeah!");
    }

    public void RemovePowerup()
    {
        playerController.onHammerBounce.RemoveListener(Test);
        powerupManager.RemoveCurrentPowerup();
        Destroy(this);
    }
}
