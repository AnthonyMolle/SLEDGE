using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    PlayerSaveData.Grade timeGrade, killGrade, styleGrade, finalGrade, collectableGrade;
    private RectTransform RectTransform;

    private bool gradesDisplayed = false;

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
        kills = "kills: " + ScoreManager.Instance.GetEnemiesKilled().ToString() + "/" + ScoreManager.Instance.GetMaxEnemies().ToString();
        styleKills = "style: " + ScoreManager.Instance.GetStyleKills().ToString();
        collectables_found = "Collectables: " + ScoreManager.Instance.GetCollectible().ToString() + "/" + ScoreManager.Instance.GetMaxCollectibles().ToString();

        // Calculate Grades
        #region Time Grade
        float endTime = ScoreManager.Instance.GetCurrentTime();
        if (endTime < ScoreManager.Instance.GetTimeThreshold(PlayerSaveData.Grade.S))
        {
            timeGrade = PlayerSaveData.Grade.S;
        }
        else if (endTime < ScoreManager.Instance.GetTimeThreshold(PlayerSaveData.Grade.A))
        {
            timeGrade = PlayerSaveData.Grade.A;
        }
        else if (endTime < ScoreManager.Instance.GetTimeThreshold(PlayerSaveData.Grade.B))
        {
            timeGrade = PlayerSaveData.Grade.B;
        }
        else if (endTime < ScoreManager.Instance.GetTimeThreshold(PlayerSaveData.Grade.C))
        {
            timeGrade = PlayerSaveData.Grade.C;
        }
        else
        {
            timeGrade = PlayerSaveData.Grade.D;
        }
        #endregion

        #region Kill Grade
        if (ScoreManager.Instance.GetMaxEnemies() == 0)
        {
            killGrade = PlayerSaveData.Grade.S;
        }
        else
        {
            float killPercent = ScoreManager.Instance.GetEnemiesKilled() / ScoreManager.Instance.GetMaxEnemies();
            if (killPercent >= 1)
            {
                killGrade = PlayerSaveData.Grade.S;
            }
            else if (killPercent >= 0.9)
            {
                killGrade = PlayerSaveData.Grade.A;
            }
            else if (killPercent >= 0.7)
            {
                killGrade = PlayerSaveData.Grade.B;
            }
            else if (killPercent >= 0.5)
            {
                killGrade = PlayerSaveData.Grade.C;
            }
            else
            {
                killGrade = PlayerSaveData.Grade.D;
            }
        }
        #endregion

        #region Style Grade
        if (ScoreManager.Instance.GetMaxStyle() == 0)
        {
            styleGrade = PlayerSaveData.Grade.S;
        }
        else
        {
            float stylePercent = ScoreManager.Instance.GetStyleKills() / ScoreManager.Instance.GetMaxStyle();
            if (stylePercent >= 1)
            {
                styleGrade = PlayerSaveData.Grade.S;
            }
            else if (stylePercent >= 0.9)
            {
                styleGrade = PlayerSaveData.Grade.A;
            }
            else if (stylePercent >= 0.7)
            {
                styleGrade = PlayerSaveData.Grade.B;
            }
            else if (stylePercent >= 0.5)
            {
                styleGrade = PlayerSaveData.Grade.C;
            }
            else
            {
                styleGrade = PlayerSaveData.Grade.D;
            }
        }
        #endregion

        #region Final Grade
        int finalPoints = PlayerSaveData.Instance.GetPointsFromRank(timeGrade) + PlayerSaveData.Instance.GetPointsFromRank(killGrade) + PlayerSaveData.Instance.GetPointsFromRank(styleGrade);
        if (finalPoints >= 15)
        {
            finalGrade = PlayerSaveData.Grade.S;
        }
        else if (finalPoints >= 11)
        {
            finalGrade = PlayerSaveData.Grade.A;
        }
        else if (finalPoints >= 8)
        {
            finalGrade = PlayerSaveData.Grade.B;
        }
        else if (finalPoints >= 5)
        {
            finalGrade = PlayerSaveData.Grade.C;
        }
        else
        {
            finalGrade = PlayerSaveData.Grade.D;
        }
        #endregion

        // Level Completion Analytics
        if (DataCollection.Instance != null)
        {
            DataCollection.Instance.RecordLevelCompleteEvent(SceneManager.GetActiveScene().name, ScoreManager.Instance.GetCurrentTime(), GradeToString(finalGrade));
        }

        // Save high scores
        PlayerSaveData.Instance.SaveLevelData(SceneManager.GetActiveScene().name, ScoreManager.Instance.GetCurrentTime(), finalGrade, ScoreManager.Instance.GetCollectible());
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

    private string GradeToString(PlayerSaveData.Grade g)
    {
        switch (g)
        {
            case PlayerSaveData.Grade.A:
                return "A";
            case PlayerSaveData.Grade.B:
                return "B";
            case PlayerSaveData.Grade.C:
                return "C";
            case PlayerSaveData.Grade.D:
                return "D";
            case PlayerSaveData.Grade.S:
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


