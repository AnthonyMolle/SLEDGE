using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class IntroTarget : MonoBehaviour
{
    GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;

        // As soon as the level begins, the player starts the level by facing and being launched toward our level target
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.LookAt(gameObject.transform);
        player.GetComponent<Rigidbody>().AddForce(player.transform.forward * 25f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
