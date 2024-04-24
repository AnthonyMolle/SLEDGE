using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    public EndScreenScore Time;
    public EndScreenScore Kills;
    public EndScreenScore StyleKills;

    public TextMeshProUGUI finalGradeText;
    public GameObject dividerTwo;


    public bool triggerDropIn;
    private bool triggerReveal;

    public float dropSpeed = 4f;

    public float secBetweenReveals = 0.5f;
    public float secBetweenScoreAndGrade = 0.5f;
    public float secMultiplier = 0.65f;

    string stageTime, kills, styleKills;
    Grade timeGrade, killGrade, styleGrade, finalGrade;
    private RectTransform RectTransform;

    private bool gradesDisplayed = false;

    public enum Grade
    {
        F, // 0
        D, // 1
        C, // 2
        B, // 3
        A  // 4
    }
    private void Start()
    {
        finalGradeText.text = "";
        dividerTwo.SetActive(false);
        RectTransform = GetComponent<RectTransform>();
        RectTransform.sizeDelta = new Vector2(640.3f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerDropIn)
        {
            Vector2 sizeDelta = RectTransform.sizeDelta;

            sizeDelta = new Vector2(640.3f, Mathf.Lerp(sizeDelta.y, 1500, dropSpeed * UnityEngine.Time.deltaTime));

            if(sizeDelta.y >= 1420)
            {
                triggerReveal = true;
                triggerDropIn = false;
            }

            RectTransform.sizeDelta = sizeDelta;
        }

        if (triggerReveal)
        {
            

            triggerReveal = false;

            setupData();

            // Match expectations of time
            secBetweenReveals += secBetweenScoreAndGrade;

            StartCoroutine(reveal());
        }
    }

    private void setupData()
    {
        // Get Scores
        stageTime = ScoreManager.Instance.GetCurrentTime().ToString();
        kills = "kills: " + ScoreManager.Instance.GetEnemiesKilled().ToString();
        styleKills = "styles: " + ScoreManager.Instance.GetStyleKills().ToString();

        // Calculate Grades
        timeGrade = Grade.F;
        killGrade = Grade.F;
        styleGrade = Grade.C;
        finalGrade = Grade.F;
    }

    IEnumerator reveal()
    {

        // Reveal Time
        Time.triggerReveal(secBetweenScoreAndGrade, stageTime, GradeToString(timeGrade));

            yield return new WaitForSeconds(secBetweenReveals);

        // Reveal Kills
        Kills.triggerReveal(secBetweenScoreAndGrade * secMultiplier, kills, GradeToString(killGrade));

            yield return new WaitForSeconds(secBetweenReveals * secMultiplier);

        // Reveal Style Kills
        StyleKills.triggerReveal(secBetweenScoreAndGrade * secMultiplier * secMultiplier, styleKills, GradeToString(styleGrade));

            yield return new WaitForSeconds(secBetweenReveals * secMultiplier * secMultiplier);

        // Show final grade
        dividerTwo.SetActive(true);
        finalGradeText.text = GradeToString(finalGrade);

        gradesDisplayed = true;
    }

    private string GradeToString(Grade g)
    {
        switch (g)
        {
            case Grade.A:
                return "A";
            case Grade.B:
                return "B";
            case Grade.C:
                return "C";
            case Grade.D:
                return "D";
            case Grade.F:
                return "F";
            default:
                return "NAN";
        }
    }

    public bool FinishedAnimation()
    {
        return gradesDisplayed;
    }

    public void skipAnim()
    {
        StopCoroutine(reveal());
        setupData();

        RectTransform.sizeDelta = new Vector2(640.3f, 1500);
        Time.triggerReveal(0, stageTime, GradeToString(timeGrade));
        Kills.triggerReveal(0, kills, GradeToString(killGrade));
        StyleKills.triggerReveal(0, styleKills, GradeToString(styleGrade));

        dividerTwo.SetActive(true);
        finalGradeText.text = GradeToString(finalGrade);

        gradesDisplayed = true;

    }

}


