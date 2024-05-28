using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupBase : MonoBehaviour
{
    [SerializeField]
    GameObject powerup;

    public float powerupReturnTime;

    public Image timerFill;
    
    float timer;

    bool timerRunning;

    void Start()
    {
        ResetTimer();
    }
    public void CollectPowerup()
    {
        powerup.SetActive(false);
        timerRunning = true;
        timerFill.enabled = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            timerFill.fillAmount = timer / powerupReturnTime;

            if (timer <= 0)
            {
                timerRunning = false;
                ResetTimer();
                powerup.SetActive(true);
                timerFill.enabled = false;
            }
        }
    }

    void ResetTimer()
    {
        timer = powerupReturnTime;
    }
}
