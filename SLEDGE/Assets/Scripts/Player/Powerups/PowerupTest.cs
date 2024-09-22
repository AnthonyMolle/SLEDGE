using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupTest : MonoBehaviour
{
    PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.onHammerBounce.AddListener(Test);
    }

    void Test()
    {
        Debug.Log("Fuck yeah!");
    }

    public void RemovePowerup()
    {
        playerController.onHammerBounce.RemoveListener(Test);
        Destroy(this);
    }
}
