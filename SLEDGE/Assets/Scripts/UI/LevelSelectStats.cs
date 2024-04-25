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

    // Start is called before the first frame update
    void Start()
    {
        timeText.text = "Best Time: N/A";
        rankText.text = "Best Rank: N/A";
        collectibleText.text = "Collectibles: N/A";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
