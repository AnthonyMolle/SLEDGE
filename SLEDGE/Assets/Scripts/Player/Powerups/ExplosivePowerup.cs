using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePowerup : MonoBehaviour
{
    PlayerController playerController;

    [Tooltip("How much additional force does this powerup provide?")]
    public float forceAddend = 25f;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.onHammerBounce.AddListener(RemovePowerup);
        playerController.IncreaseInitialForce(forceAddend);
    }

    public void RemovePowerup()
    {
        playerController.IncreaseInitialForce(-forceAddend);
        playerController.onHammerBounce.RemoveListener(RemovePowerup);
        Destroy(this);
    }
}
