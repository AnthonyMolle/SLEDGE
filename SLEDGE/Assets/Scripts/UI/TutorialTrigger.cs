using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string tutorialText;
    [Range(0f, 1000f)]
    public float captionWidth;
    [Range(0f, 1000f)]
    public float captionHeight;

    void OnTriggerEnter(Collider other)
    {
        TutorialCaption.instance.ShowCaption(tutorialText, captionWidth, captionHeight);
    }

    void OnTriggerExit(Collider other)
    {
        TutorialCaption.instance.HideCaption();
    }
}
