using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteScreen : MonoBehaviour
{
    public GameObject portal;

    public EndScreenManager EndScreenManager;

    private bool AllowInput = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        EndScreenManager = GetComponentInChildren<EndScreenManager>(true);
    }

    public void StartAnimation()
    {
        //EndScreenManager.StartDropIn();
        AllowInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("allow input " + AllowInput);
        if (AllowInput && Input.anyKeyDown)
        {
            //Debug.Log("input allowed and detected");
            if (EndScreenManager.FinishedAnimation() == false) {
                EndScreenManager.skipAnim();
                //Debug.Log("skipping anim");
            }
            else
            {
                if (GameObject.Find("EndPlatform") != null)
                {
                    GameObject.Find("EndPlatform").GetComponent<EndPlatform>().GoToScene();
                    //Debug.Log("trying to go to scene");
                }
                else // REMOVE ONCE ALL OLD PORTALS ARE PHASED OUT
                {
                    portal.GetComponent<EndPortal>().GoToScene();
                }
                
            }
        }
    }
}
