using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    ScoreManager ScoreManager;
    
    // Start is called before the first frame update
    void Start()
    {
        ScoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collectible found!");
            if (ScoreManager != null)
            {
                ScoreManager.AddCollectible(gameObject);
            }
            Destroy(gameObject);
        }
    }
}
