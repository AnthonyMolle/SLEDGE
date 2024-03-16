using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteScreen : MonoBehaviour
{
    public GameObject portal;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            portal.GetComponent<EndPortal>().GoToScene();
        }
    }
}
