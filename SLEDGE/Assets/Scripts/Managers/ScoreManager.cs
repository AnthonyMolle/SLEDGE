using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Acts as a data aggregate for other objects to use
// Victory screen and PlayerSaveData get data from this
// Other script write to this data
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    // Data links
    public Speed_Run_Timer timer;

    private void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    #region Collectibles
    public int MaxCollectibles;
    int CollectiblesFound;

    public int GetCollectible() { return CollectiblesFound; }
    public void AddCollectible(GameObject collectible)
    {
        // Possibly track specific collectibles later?
        CollectiblesFound++;
        Debug.Log("Collectibles found: " + CollectiblesFound);
    }

    #endregion

    #region Current Time
    public float GetCurrentTime() { return timer.GetTimeFloat(); }
    public string GetPrintableTime() { return timer.GetTimeString();  }
    #endregion

    #region Combat
    int EnemiesKilled;
    int StyleKills;
    float DamageTaken;

    public int GetEnemiesKilled() { return EnemiesKilled; }
    public int GetStyleKills() { return StyleKills; }
    public float GetDamageTaken() { return DamageTaken; }

    public void AddEnemiesKilled(int _enemiesKilled) { EnemiesKilled += _enemiesKilled; }
    public void AddStyleKills(int _styleKills) { StyleKills += _styleKills; }
    public void AddDamageTaken(float _damageTaken) { DamageTaken += _damageTaken; }
    #endregion

    #region Movement
    int TimesLanded;

    public int GetTimesLanded() { return TimesLanded; }
    public void AddTimesLanded(int _timesLanded) { TimesLanded += _timesLanded; }
    #endregion
}
