using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSign : MonoBehaviour
{
    //should pause time while reading but I'm not touching that code lmao
    //Was not able to turn off canvases from the Inspector? So....

    public GameObject parent;
    public GameObject worldCanvas;
    public GameObject pressE; //text obj saying press E
    public GameObject questionMark;
    public GameObject tutorialCanvas;
    bool inTrigger = false; //literally any rigidbody could be in the trigger. Enemies prolly count
    bool ePressed;

    // Start is called before the first frame update
    void Start()
    {
        Canvas[] canvases = parent.GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
        }
        worldCanvas.SetActive(true);
        pressE.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void OnTriggerEnter(Collider other)
    {
        pressE.SetActive(true);
        questionMark.SetActive(false);
        inTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        inTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        pressE.SetActive(false);
        questionMark.SetActive(true);
        inTrigger = false;
    }

    void HandleInput()
    {
        //Debug.Log("handling input");
        //Debug.Log("e " + Input.GetKeyDown(KeyCode.E));
        //Debug.Log(tutorialCanvas.activeSelf + "tutCanvasActiveness");

        if(Input.GetKeyDown(KeyCode.E))
            Debug.Log("e " + Input.GetKeyDown(KeyCode.E));

        if (inTrigger && Input.GetKeyDown(KeyCode.E) && !tutorialCanvas.activeSelf) //if press E next to sign and tutorial is not already active
        {
            //Debug.Log("Should be setting active");
            tutorialCanvas.SetActive(true);
            //Debug.Log(tutorialCanvas.activeSelf + "tutCanvasActiveness");
        } else if (Input.GetKeyDown(KeyCode.E) && tutorialCanvas.activeSelf) //if press E and tutorial is already active
        {
            //Debug.Log("Should not be setting active");
            tutorialCanvas.SetActive(false);
        }
    }

}
