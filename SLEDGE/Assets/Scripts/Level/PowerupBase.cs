using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBase : MonoBehaviour
{
    [SerializeField]
    GameObject powerup;

    public float powerupReturnTime;
    public void CollectPowerup()
    {
        StartCoroutine(ReenablePowerup());
    }

    IEnumerator ReenablePowerup()
    {
        powerup.SetActive(false);
        yield return new WaitForSeconds(powerupReturnTime);
        powerup.SetActive(true);
    }
}
