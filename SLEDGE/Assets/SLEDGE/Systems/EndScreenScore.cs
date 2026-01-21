using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class EndScreenScore : MonoBehaviour
{

    private string grade;
    public TextMeshProUGUI text;
    public TextMeshProUGUI rating;

    public void Awake()
    {
        //setVisible(false);
        rating.text = "-";
    }

    // Call to trigger our reveal of the score
    // Returns true when done
    public void triggerReveal(float secBeforeGrade, string statToDisplay, string passedGrade)
    {
        if (!text || !rating) return;

        // Save stats
        text.text = statToDisplay;
        grade = passedGrade;

        // Start Reveal
        StartCoroutine(reveal(secBeforeGrade));
    }

    IEnumerator reveal(float seconds)
    {
        // Show stats
        //setVisible(true);

        // Wait (Dramatic Pause)
        yield return new WaitForSecondsRealtime(seconds);

        // Show grade
        rating.text = grade;
    }

    private void setVisible(bool visable)
    {
        int alpha = visable ? 1 : 0;
        GetComponent<CanvasRenderer>().SetAlpha(alpha);
        foreach (CanvasRenderer x in GetComponentsInChildren<CanvasRenderer>())
        {
            x.SetAlpha(alpha);
        }
    }
}
