using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Crack : MonoBehaviour
{
    public Image crack;
    public Image splash;
    public List<Sprite> cracks = new List<Sprite>();
    public List<Sprite> splashes = new List<Sprite>();
    public int crackAmt = 0;

    void OnValidate()
    {
        SetCrack(crackAmt);
    }

    public void SetCrack(int num)
    {
        if (num == 0)
        {
            crack.color = SetAlpha(crack.color, 0);
            splash.color = SetAlpha(splash.color, 0);
            return;
        }
        crack.color = SetAlpha(crack.color, 1);
        splash.color = SetAlpha(splash.color, 1);
        crack.sprite = cracks[num-1];
        splash.sprite = splashes[num-1];
    }

    private Color SetAlpha(Color c, float a)
    {
        c = new Color(c.r, c.g, c.b, a);
        return c;
    }
}
