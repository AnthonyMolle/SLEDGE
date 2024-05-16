using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    public GameObject playerSaveData;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerSaveData.Instance == null)
        {
            Instantiate(playerSaveData);
        }
        // Load data from file eventually
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
