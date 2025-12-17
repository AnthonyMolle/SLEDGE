using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    public int health = 3;
    public List<Heart> hearts = new List<Heart>();

    public void SetHealth(int h)
    {
        health = h-1;
        for (int i = 0; i < hearts.Count; i++)
        {
            // if (i < health-1)
            // {
            //     hearts[i].Damage();
            // }
            hearts[i].Heal();
            if (i == health)
            {
                hearts[i].Pump();
            }
            if (i > health)
            {
                hearts[i].Damage();
            }
            // else
            // {
            //     hearts[i].Heal();
            // }
        }
    }
}
