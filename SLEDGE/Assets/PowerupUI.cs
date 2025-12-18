using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Powerup = PlayerController.Powerup;

public class PowerupUI : MonoBehaviour
{
    public List<Image> powerupIcons = new List<Image>();
    public Sprite airburstSprite;
    public Sprite explosionSprite;
    public Powerup pup;

    void Start()
    {
        SetPowerup(Powerup.None);
    }

    public void SetPowerup(Powerup p)
    {
        pup = p;
        if (p == Powerup.None) {
            powerupIcons[0].gameObject.SetActive(false);
            return;
        }
        powerupIcons[0].gameObject.SetActive(true);
        if (p == Powerup.Airburst) {
            powerupIcons[0].sprite = airburstSprite;
        }
        else if (p == Powerup.Explosive) {
            powerupIcons[0].sprite = explosionSprite;
        }
        else {
            powerupIcons[0].enabled = false;
        }
    }

    [ContextMenu("Set Powerup to Airburst")]
    public void SetPowerupToAirburst()
    {
        SetPowerup(Powerup.Airburst);
    }

    [ContextMenu("Set Powerup to Explosive")]
    public void SetPowerupToExplosive()
    {
        SetPowerup(Powerup.Explosive);
    }

    [ContextMenu("Set Powerup to None")]
    public void SetPowerupToNone()
    {
        SetPowerup(Powerup.None);
    }
}
