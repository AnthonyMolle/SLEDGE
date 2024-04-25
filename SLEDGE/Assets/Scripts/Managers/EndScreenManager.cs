using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public EndScreenScore Time;
    public EndScreenScore Kills;
    public EndScreenScore StyleKills;
    public EndScreenScore Collectables;

    public TextMeshProUGUI finalGradeText;
    public GameObject dividerTwo;


    private bool triggerDropIn;
    private bool triggerReveal;
    private bool waitingForAnim;

    public float dropSpeed = 4f;

    public float secBetweenReveals = 0.5f;
    public float secBetweenScoreAndGrade = 0.5f;
    public float secMultiplier = 0.65f;

    string stageTime, kills, styleKills, collectables_found;
    Grade timeGrade, killGrade, styleGrade, finalGrade, collectableGrade;
    private RectTransform RectTransform;

    private bool gradesDisplayed = false;


    public enum Grade
    {     
        D, // 0
        C, // 1
        B, // 2
        A, // 3
        S  // 4
    }
    private void Start()
    {
        finalGradeText.text = "";
        dividerTwo.SetActive(false);
        RectTransform = GetComponent<RectTransform>();
        waitingForAnim = false;
        RectTransform.sizeDelta = new Vector2(640.3f, 0);
    }

    public void StartDropIn()
    {
        GetComponent<Animator>().SetBool("DropIn",true);
    }

    // Update is called once per frame
    void Update()
    {
        if (RectTransform.sizeDelta.y >= 1420 && triggerReveal == false)
        {
            triggerReveal = true;

            setupData();

            // Match expectations of time
            secBetweenReveals += secBetweenScoreAndGrade;

            StartCoroutine(reveal());
        }
    }

    private void setupData()
    {
        // Get Scores
        stageTime = ScoreManager.Instance.GetPrintableTime();
        kills = "kills: " + ScoreManager.Instance.GetEnemiesKilled().ToString();
        styleKills = "styles: " + ScoreManager.Instance.GetStyleKills().ToString();
        collectables_found = "Collectables: " + ScoreManager.Instance.GetCollectible().ToString();

        // Calculate Grades
        timeGrade = Grade.A;
        killGrade = Grade.A;
        styleGrade = Grade.A;
        finalGrade = Grade.A;

        PlayerSaveData.Instance.SaveLevelData(SceneManager.GetActiveScene().name, ScoreManager.Instance.GetCurrentTime(), PlayerSaveData.Grade.A, ScoreManager.Instance.GetCollectible());
    }

    IEnumerator reveal()
    {

        // Reveal Time
        Time.triggerReveal(secBetweenScoreAndGrade, stageTime, GradeToString(timeGrade));

            yield return new WaitForSecondsRealtime(secBetweenReveals);

        // Reveal Kills
        Kills.triggerReveal(secBetweenScoreAndGrade * secMultiplier, kills, GradeToString(killGrade));

            yield return new WaitForSecondsRealtime(secBetweenReveals * secMultiplier);

        // Reveal Style Kills
        StyleKills.triggerReveal(secBetweenScoreAndGrade * secMultiplier * secMultiplier, styleKills, GradeToString(styleGrade));

            yield return new WaitForSecondsRealtime(secBetweenReveals * secMultiplier * secMultiplier);

        // Show final grade
        dividerTwo.SetActive(true);
        finalGradeText.text = GradeToString(finalGrade);

        Collectables.triggerReveal(0, collectables_found,"");

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
            case Grade.S:
                return "S";
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
        Collectables.triggerReveal(0, collectables_found, GradeToString(collectableGrade));

        dividerTwo.SetActive(true);
        finalGradeText.text = GradeToString(finalGrade);

        gradesDisplayed = true;

    }

}


