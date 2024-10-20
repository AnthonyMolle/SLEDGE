using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteScreen : MonoBehaviour
{
    public GameObject portal;

    private EndScreenManager EndScreenManager;

    private bool AllowInput = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        EndScreenManager = GetComponentInChildren<EndScreenManager>();
    }

    public void StartAnimation()
    {
        EndScreenManager.StartDropIn();
        AllowInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (AllowInput && Input.anyKeyDown)
        {
            if(EndScreenManager.FinishedAnimation() == false) {
                EndScreenManager.skipAnim();
            }
            else
            {
                if (GameObject.Find("EndPlatform") != null)
                {
                    GameObject.Find("EndPlatform").GetComponent<EndPlatform>().GoToScene();
                }
                else // REMOVE ONCE ALL OLD PORTALS ARE PHASED OUT
                {
                    portal.GetComponent<EndPortal>().GoToScene();
                }
                
            }
        }
    }
}
