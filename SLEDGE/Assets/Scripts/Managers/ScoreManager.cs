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
    public int MaxCollectibles { get; set; }
    public int CollectiblesFound { get; set; }
    #endregion

    #region Speedrunning
    public float CurrentTime {
        // Avoids updating timer every frame in two places
        get
        {
            return timer.GetTimeFloat();
        }
    }
    #endregion

    #region Combat
    public int EnemiesKilled { get; set; }
    public int StyleKills { get; set; }
    public float DamageTaken { get; set; }
    #endregion

    #region Movement
    public int TimesLanded { get; set; }
    #endregion

    public void PickUpCollectible(GameObject collectible)
    {
        // Possibly track specific collectibles later?
        CollectiblesFound++;
        Debug.Log("Collectibles found: " + CollectiblesFound);
    }
}
