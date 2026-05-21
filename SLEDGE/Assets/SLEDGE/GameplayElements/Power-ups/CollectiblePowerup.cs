using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectiblePowerup : MonoBehaviour
{
    [SerializeField]
    UnityEvent onPickup;

    PlayerController playerController;

    public PlayerController.Powerup newPowerup;

    void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.CollectPowerup(newPowerup);
            onPickup.Invoke();
        }
    }
}
