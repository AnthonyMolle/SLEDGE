using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static PlayerController;

public class PowerupManager : MonoBehaviour
{
    string currentPowerup;

    [SerializeField]
    TextMeshProUGUI powerupText;

    public void AddPowerup(string newPowerup)
    {
        if (currentPowerup != null)
        {
            System.Type scriptType = System.Type.GetType(currentPowerup + ",Assembly-CSharp");
            Destroy(GetComponent(scriptType));
        }
        currentPowerup = newPowerup;
        powerupText.text = "Active Powerup: " + currentPowerup;
    }

    public void RemoveCurrentPowerup()
    {
        currentPowerup = null;
    }
}
