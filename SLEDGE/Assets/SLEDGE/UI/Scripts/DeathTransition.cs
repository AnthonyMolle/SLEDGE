using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTransition : MonoBehaviour
{
    GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    /*
    void Update()
    {
        if (Input.anyKeyDown)
        {
            player.GetComponent<PlayerController>().ResetPlayer();
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }
    */
}
