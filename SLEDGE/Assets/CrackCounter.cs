using UnityEngine;
using System.Collections.Generic;

public class CrackCounter : MonoBehaviour
{
    public int crackLevel = 0;
    public List<Crack> cracks = new List<Crack>();
    public int crackMax = 3;


    void OnValidate()
    {
        SetCrackLevel(crackLevel);
    }
    
    public void SetCrackLevel(int level)
    {
        crackLevel = level;
        foreach (Crack crack in cracks)
        {
            crack.SetCrack(crackLevel);
        }
    }

    public void SetCrackLevelReverse(int level)
    {
        // As level counts down from crackMax to 1, crack level goes from 0 to crackMax
        int calculatedLevel = crackMax - level;
        if (calculatedLevel < 0)
        {
            calculatedLevel = 0;
        }
        if (calculatedLevel > crackMax)
        {
            calculatedLevel = crackMax;
        }
        SetCrackLevel(calculatedLevel);
    }
}