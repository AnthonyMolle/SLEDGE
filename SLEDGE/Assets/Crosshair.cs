using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Animator anim;
    public float animSpeed = 0.2f;

    void OnValidate()
    {
        anim.speed = animSpeed;
    }
    public void Charge()
    {
        anim.SetBool("Charge", true);
    }

    public void CancelCharge()
    {
        anim.SetBool("Charge", false);
    }

    public void Slam()
    {
        anim.SetBool("Slam", true);
        anim.SetBool("Charge", false);
    }
}
