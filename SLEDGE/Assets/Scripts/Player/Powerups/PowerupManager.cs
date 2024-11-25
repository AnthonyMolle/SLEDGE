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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwapPowerups();
        }
    }

    public void AddPowerup(string newPowerup)
    {
        if (currentPowerup != null)
        {
            Type scriptType = Type.GetType(currentPowerup + ",Assembly-CSharp");
            Destroy(GetComponent(scriptType));
        }
        currentPowerup = newPowerup;
        powerupText.text = "Active Powerup: " + currentPowerup;
    }

    void SwapPowerups()
    {

    }

    public void RemoveCurrentPowerup()
    {
        currentPowerup = null;
    }
}
