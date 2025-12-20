using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSign : MonoBehaviour
{
    //should pause time while reading but I'm not touching that code lmao
    //Was not able to turn off canvases from the Inspector? So....

    public GameObject interactionCanvas;
    public GameObject tutorialCanvas; //you can change the default in the prefabs
    public GameObject parent;
    bool inTrigger = false; //literally any rigidbody could be in the trigger. Enemies prolly count
    bool ePressed;

    // Start is called before the first frame update
    void Start()
    {
        Canvas[] canvases = parent.GetComponentsInChildren<Canvas>();
        Debug.Log(canvases);
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
            Debug.Log("canvas deactivate");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void OnTriggerEnter(Collider other)
    {
        interactionCanvas.SetActive(true);
        inTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        inTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        interactionCanvas.SetActive(false);
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
