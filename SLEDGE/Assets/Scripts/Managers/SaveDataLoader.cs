using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    public GameObject playerSaveData;
    public GameObject dataCollection;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
