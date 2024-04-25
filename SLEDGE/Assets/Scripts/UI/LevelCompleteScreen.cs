using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteScreen : MonoBehaviour
{
    public GameObject portal;

    private EndScreenManager EndScreenManager;
    
    // Start is called before the first frame update
    void Start()
    {
        EndScreenManager = GetComponentInChildren<EndScreenManager>();
    }

    public void StartAnimation()
    {
        EndScreenManager.StartDropIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if(EndScreenManager.FinishedAnimation() == false) {
                EndScreenManager.skipAnim();
            }
            else
            {
                portal.GetComponent<EndPortal>().GoToScene();
            }
        }
    }
}
