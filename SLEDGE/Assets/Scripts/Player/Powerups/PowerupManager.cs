using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerupManager : MonoBehaviour
{
    public int maxPowerups;
    int numPowerups;

    List<string> currentPowerups = new List<string>();
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
        /*
        if (currentPowerup != null)
        {
            Type scriptType = Type.GetType(currentPowerup + ",Assembly-CSharp");
            Destroy(GetComponent(scriptType));
        }
        numPowerups++;
        currentPowerup = newPowerup;
        powerupText.text = "Active Powerup: " + currentPowerup;
        */

        if (numPowerups <= maxPowerups)
        {
            numPowerups++;
            currentPowerups.Add(newPowerup);
            if (numPowerups == 1)
            {
                currentPowerup = newPowerup;
            }
            SetText();
        }
    }

    void SwapPowerups()
    {
        if (currentPowerup == currentPowerups[0])
        {
            currentPowerup = currentPowerups[1];
        }
        else
        {
            currentPowerup = currentPowerups[0];
        }
        SetText();
    }

    public bool IsCurrentPowerup(string name)
    {
        return name == currentPowerup;
    }

    public void UsePowerup()
    {
        currentPowerup.Remove(currentPowerup.IndexOf(currentPowerup));
        Destroy(GetComponent(GetPowerupType()));
        SetText();
    }

    Type GetPowerupType()
    {
        return Type.GetType(currentPowerup + ",Assembly-CSharp");
    }

    void SetText()
    {
        powerupText.text = "Active Powerup: " + currentPowerup;
    }

    //This method is set to be deprecated soon.
    public void RemoveCurrentPowerup()
    {
        currentPowerup = null;
    }
}
