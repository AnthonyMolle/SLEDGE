using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public bool pumping = false;
    public bool damaged = false;
    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        if (pumping) {
            anim.SetBool("Pump", true);
        }
        if (damaged) {
            anim.SetBool("Dead", true);
        }
    }

    void OnValidate()
    {
        if (damaged) {
            Damage();
        }
        else {
            Heal();
        }
        if (pumping) {
            Pump();
        }
        else {
            StopPumping();
        }
    }

    public void Pump() 
    {
        pumping = true;
        damaged = false;
        anim.SetBool("Pump", true);
        anim.SetBool("Dead", false);
    }

    public void StopPumping() 
    {
        pumping = false;
        anim.SetBool("Pump", false);
    }

    public void Damage() 
    {
        pumping = false;
        damaged = true;
        anim.SetBool("Pump", false);
        anim.SetBool("Dead", true);
    }

    public void Heal() 
    {
        pumping = false;
        damaged = false;
        anim.SetBool("Dead", false);
        anim.SetBool("Pump", false);
    }

    [ContextMenu("Revive")]
    public void Revive()
    {
        Heal();
        Pump();
    }
}
