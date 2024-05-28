using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    public static PlayerSaveData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public enum Grade
    {
        D = 0, // 0
        C = 1, // 1
        B = 2, // 2
        A = 3, // 3
        S = 4  // 4
    }

    public int GetPointsFromRank(Grade rank)
    {
        switch (rank) 
        {
            case Grade.S:
                return 5;
            case Grade.A:
                return 4;
            case Grade.B:
                return 3;
            case Grade.C:
                return 2;
            case Grade.D:
                return 1;
            default: return 1;
        }
    }

    struct LevelStats
    {
        public float time;
        public Grade grade;
        public int collectibles;

        public LevelStats(float time, Grade grade, int collectibles)
        {
            this.time = time;
            this.grade = grade;
            this.collectibles = collectibles;
        }

        public void setTime(float time) { this.time = time; }

        public void setGrade(Grade grade) { this.grade = grade; }

        public void setCollectibles(int collectibles) { this.collectibles = collectibles; }
    }

    Dictionary<string, LevelStats> LevelsData = new Dictionary<string, LevelStats>();

    // To be called at the end of a level, saves that level's stats if there is no previous data or if those new stats are improvements
    public void SaveLevelData(string level, float time, Grade grade, int collectibles)
    {   
        if (!LevelsData.ContainsKey(level))
        {
            // If this is our first time saving for this level, just save all the data
            LevelsData[level] = new LevelStats(time, grade, collectibles);
        }
        else
        {
            // If we already have saved stats for this level, check to see if any of the new ones are improvements
            if (LevelsData[level].time > time)
            {
                LevelsData[level].setTime(time);
            }
            if (LevelsData[level].grade < grade)
            {
                LevelsData[level].setGrade(grade);
            }
            if (LevelsData[level].collectibles < collectibles)
            {
                LevelsData[level].setCollectibles(collectibles);
            }
        }

        // Save level data to file
        string json = JsonConvert.SerializeObject(LevelsData);

        using(StreamWriter writer = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + "LevelData.json", false))
        {
            writer.Write(json);
        }
    }

    public void LoadLevelData(string path)
    {
        string json = string.Empty;

        using(StreamReader reader = new StreamReader(path))
        {
            json = reader.ReadToEnd();
        }

        Dictionary<string, LevelStats> data = JsonConvert.DeserializeObject<Dictionary<string, LevelStats>>(json);

        foreach (KeyValuePair<string, LevelStats> pair in data)
        {
            LevelsData[pair.Key] = pair.Value;
        }

    }

    public bool ContainsLevelData(string level) {  return LevelsData.ContainsKey(level); }

    public float GetLevelTime(string level) { return LevelsData[level].time; }

    public string GetLevelGrade(string level) { return LevelsData[level].grade.ToString(); }

    public int GetLevelCollectibles(string level) { return LevelsData[level].collectibles; }
}
