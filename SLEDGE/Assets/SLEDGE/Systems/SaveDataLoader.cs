using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    public GameObject playerSaveData;
    public GameObject dataCollection;

    public CanvasGroup ConsentPrompt;
    UIManager UImanager;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerSaveData.Instance == null)
        {
            Instantiate(playerSaveData);
        }
        string path = Application.dataPath + Path.AltDirectorySeparatorChar + "LevelData.json";
        if (File.Exists(path) && new FileInfo(path).Length != 0)
        {
            PlayerSaveData.Instance.LoadLevelData(path);
        }

        /*
        if (DataCollection.Instance == null)
        {
            Instantiate(dataCollection);
        }


        // Ask for player consent for data collection (only if they havent been asked before)
        int enableDataCollection = PlayerPrefs.GetInt("EnableDataCollection", -1);
        if (enableDataCollection == -1)
        {
            PlayerPrefs.SetInt("EnableDataCollection", 0);
            PlayerPrefs.Save();
            UImanager = GameObject.Find("Main Canvas").GetComponent<UIManager>();
            UImanager.TransitionTo(ConsentPrompt);
        } 
        else if (enableDataCollection == 1) 
        {
            DataCollection.Instance.StartCollection();
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
