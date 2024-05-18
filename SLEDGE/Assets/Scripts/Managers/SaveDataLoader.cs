using System.Collections;
using System.Collections.Generic;
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
        // Load data from file eventually
        if (PlayerSaveData.Instance == null)
        {
            Instantiate(playerSaveData);
        }

        if (DataCollection.Instance == null)
        {
            Instantiate(dataCollection);
        }

        // Ask for player consent for data collection
        if (!DataCollection.Instance.playerPrompted)
        {
            UImanager = GameObject.Find("Main Canvas").GetComponent<UIManager>();
            UImanager.TransitionTo(ConsentPrompt);
            DataCollection.Instance.playerPrompted = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
