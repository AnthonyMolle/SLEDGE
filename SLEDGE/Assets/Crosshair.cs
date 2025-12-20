using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Animator anim;
    public float animSpeed = 0.2f;
    public Image hitIndicator;

    void OnValidate()
    {
        anim.speed = animSpeed;
    }
    public void Charge()
    {
        anim.SetBool("Slam", false);
        anim.SetBool("Charge", true);
    }

    public void CancelCharge()
    {
        anim.SetBool("Slam", false);
        anim.SetBool("Charge", false);
    }

    public void Slam(bool hit = false, int color = 0)
    {
        anim.SetBool("Slam", true);
        anim.SetBool("Charge", false);
        if (hit)
        {
            anim.SetTrigger("Hit");
            if (color == 0)
            {
                hitIndicator.color = Color.white;
            }
            else if (color == 1)
            {
                hitIndicator.color = Color.red;
            }
            else if (color == 2)
            {
                hitIndicator.color = new Color(0.3f, 0.3f, 0.3f);
            }
        }
        else
        {
            anim.ResetTrigger("Hit");
        }
    }

    public void SwingHit(int color = 0)
    {
        anim.SetTrigger("Hit");
        if (color == 0)
        {
            hitIndicator.color = Color.white;
        }
        else if (color == 1)
        {
            hitIndicator.color = Color.red;
        }
        else if (color == 2)
        {
            hitIndicator.color = new Color(0.3f, 0.3f, 0.3f);
        }
    }

    public void ResetCrosshair()
    {
        anim.ResetTrigger("Hit");
        anim.SetBool("Slam", false);
        anim.SetBool("Charge", false);
    }
}
