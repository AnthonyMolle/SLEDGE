using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupTest : MonoBehaviour
{
    PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.onHammerHit.AddListener(Test);
    }

    void Test()
    {
        Debug.Log("Fuck yeah!");
    }

    void OnDisable()
    {
        playerController.onHammerHit.RemoveListener(Test);
    }
}
