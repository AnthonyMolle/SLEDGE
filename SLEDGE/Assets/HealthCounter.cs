using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    public int health = 3;
    public List<Heart> hearts = new List<Heart>();
    public Animator heartLabelAnim;
    public float heartLabelAnimSpeed = .12f;
    public CrackCounter cc;


    void Start()
    {
        heartLabelAnim.speed = heartLabelAnimSpeed;    
    }

    public void SetHealth(int h)
    {

        health = h-1;
        if (health+1 > heartLabelAnim.GetInteger("hp"))
        {
            if (health+1 == 3)
            {
                heartLabelAnim.Play("HL_full-hp", -1, 0f);
            }
        }
        heartLabelAnim.SetInteger("hp", health+1);
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
        if (cc != null)
        {
            if (health == 2)
            {
                cc.SetCrackLevel(0);
                return;
            }
            if (health == 1)
            {
                cc.SetCrackLevel(1);
                return;
            }
            if (health == 0)
            {
                cc.SetCrackLevel(2);
                return;
            }
            if (health < 0)
            {
                cc.SetCrackLevel(3);
                return;
            }
            // cc.SetCrackLevelReverse(health);
        }
    }
}
