using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    #region Collectibles
    public int MaxCollectibles = 1;
    int Collectibles = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCollectibles()
    {
        return Collectibles;
    }

    public void PickUpCollectible(GameObject collectible)
    {
        // Possibly track specific collectibles later?
        Collectibles++;
        Debug.Log("Collectibles found: " + Collectibles);
    }
}
