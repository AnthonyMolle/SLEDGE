using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public static DataCollection Instance { get; private set; }

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public class LevelCompleteEvent : Unity.Services.Analytics.Event
    {
        public LevelCompleteEvent() : base("levelComplete")
        {
        }
        public string LevelName { set { SetParameter("levelName", value); } }
        public float CompletionTime { set { SetParameter("completionTime", value); } }
        public string CompletionRank { set { SetParameter("completionRank", value); } }

    }

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartCollection()
    {
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log("Data Collection started");
    }

    public void RecordCollectibleEvent()
    {
        AnalyticsService.Instance.RecordEvent("collectibleFound");
    }

    public void RecordLevelStartEvent()
    {
        AnalyticsService.Instance.RecordEvent("levelStarted");
    }

    public void RecordLevelCompleteEvent(string levelName, float completionTime, string completionRank)
    {
        LevelCompleteEvent newEvent = new LevelCompleteEvent
        {
            LevelName = levelName,
            CompletionTime = completionTime,
            CompletionRank = completionRank
        };

        AnalyticsService.Instance.RecordEvent(newEvent);
    }
}
