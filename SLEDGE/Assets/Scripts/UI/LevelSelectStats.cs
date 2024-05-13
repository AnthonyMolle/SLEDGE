using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectStats : MonoBehaviour
{

    // public PlayerSaveData playerSaveData;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI collectibleText;

    public string LevelName;

    // Start is called before the first frame update
    void Start()
    {

        if (PlayerSaveData.Instance.ContainsLevelData(LevelName))
        {
            TimeSpan time = TimeSpan.FromSeconds(PlayerSaveData.Instance.GetLevelTime(LevelName));
            timeText.text = "Best Time: " + time.ToString(format: @"mm\:ss\.ff");
            rankText.text = "Best Rank: " + PlayerSaveData.Instance.GetLevelGrade(LevelName);
            collectibleText.text = "Collectibles: " + PlayerSaveData.Instance.GetLevelCollectibles(LevelName);
        }
        else
        {
            timeText.text = "Best Time: N/A";
            rankText.text = "Best Rank: N/A";
            collectibleText.text = "Collectibles: N/A";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
