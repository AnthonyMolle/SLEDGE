using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirburstPowerup : MonoBehaviour
{
    PlayerController playerController;

    [Tooltip("How many additional bounces does this powerup provide?")]
    //Add 1 more than you want, first one won't count due to the initial bounce
    public int additionalBounces = 2;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        GetComponent<PowerupManager>().AddPowerup(GetType().ToString());
        playerController.onExtraHammerHit.AddListener(RemovePowerup);
        playerController.IncreaseAdditionalBounces(additionalBounces);
    }

    public void RemovePowerup()
    {
        playerController.IncreaseAdditionalBounces(-additionalBounces);
        playerController.onExtraHammerHit.RemoveListener(RemovePowerup);
        Destroy(this);
    }
}
