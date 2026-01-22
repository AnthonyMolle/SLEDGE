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
        timer = FindAnyObjectByType<Speed_Run_Timer>();
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    #region Collectibles
    int MaxCollectibles;
    int CollectiblesFound;

    public int GetMaxCollectibles() { return MaxCollectibles; }
    public int GetCollectible() { return CollectiblesFound; }
    public void AddCollectible(GameObject collectible)
    {
        // Possibly track specific collectibles later?
        CollectiblesFound++;
    }

    #endregion

    #region Current Time
    public List<float> TimeThresholds;
    //public Dictionary<PlayerSaveData.Grade, float> TimeThresholds = new Dictionary<PlayerSaveData.Grade, float>();
    public float GetTimeThreshold(PlayerSaveData.Grade rank) { return TimeThresholds[(int)rank]; }
    public float GetCurrentTime() { return timer.GetTimeFloat(); }
    public string GetPrintableTime() { return timer.GetTimeString();  }
    #endregion

    #region Combat
    public int MaxEnemies = 0;
    int EnemiesKilled;
    float MaxStyle;
    int StyleKills;
    float DamageTaken;

    public int GetMaxEnemies() { return MaxEnemies; }
    public int GetEnemiesKilled() { return EnemiesKilled; }
    public float GetMaxStyle() { return MaxStyle; }
    public int GetStyleKills() { return StyleKills; }
    public float GetDamageTaken() { return DamageTaken; }

    public void AddEnemiesKilled(int _enemiesKilled) {
        StyleKills += 100;
        EnemiesKilled += _enemiesKilled; 
    }
    public void ResetKills(int _kills) {  EnemiesKilled = _kills; }
    public void AddStyleKills(int _styleKills) { StyleKills += _styleKills; }
    public void ResetStyle(int _style) { StyleKills = _style; }
    public void AddDamageTaken(float _damageTaken) { DamageTaken += _damageTaken; }
    #endregion

    #region Movement
    int TimesLanded;

    public int GetTimesLanded() { return TimesLanded; }
    public void AddTimesLanded(int _timesLanded) { TimesLanded += _timesLanded; }
    #endregion

    void Start()
    {
        /*
        if (DataCollection.Instance != null)
        {
            DataCollection.Instance.RecordLevelStartEvent();
        }
        */

        MaxCollectibles = GameObject.FindGameObjectsWithTag("Collectible").Length;
        //Debug.Log("Collectibles: " + MaxCollectibles);
        
        // Uncomment and make var private once we rework enemy respawns
        //MaxEnemies = GameObject.FindGameObjectsWithTag("Enemy Shooter").Length + GameObject.FindGameObjectsWithTag("Enemy Flyer").Length;
        //Debug.Log("Enemies: " + MaxEnemies);
        
        MaxStyle = Mathf.Ceil(MathF.Ceiling(MaxEnemies * 0.5f) * 500);
        //Debug.Log("Style: " + MaxStyle);
    }

}
