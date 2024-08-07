using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public float fadeDuration = 1;
    public AnimationCurve animCurve;
    public GameObject fade;

    public void TransitionTo(CanvasGroup a)
    {
        CanvasGroup currentCanvas = null;
        CanvasGroup[] b = FindObjectsOfType<CanvasGroup>();
        for (int i = 0; i < b.Length; i++)
        {
            if (b[i].alpha == 1)
            {
                currentCanvas = b[i];
            }
        }

        StartCoroutine(Fade(a,0,1));
        if (currentCanvas != null)
        {
            StartCoroutine(Fade(currentCanvas, 1, 0));
        }
    }

    public void TransitionToFast(CanvasGroup a)
    {
        CanvasGroup currentCanvas = null;
        CanvasGroup[] b = FindObjectsOfType<CanvasGroup>();
        for (int i = 0; i < b.Length; i++)
        {
            if (b[i].alpha == 1)
            {
                currentCanvas = b[i];
            }
        }

        a.gameObject.SetActive(true);
        if (currentCanvas != null)
        {
            currentCanvas.gameObject.SetActive(false);
        }
    }

    public void roomNext()
    {
        Instantiate(fade);
    }


    IEnumerator Fade(CanvasGroup a, float startAlpha, float endAlpha)
    {
        a.gameObject.SetActive(true);
        a.interactable = false;

        float timeElapsed = 0;

        while (timeElapsed < fadeDuration)
        {
            float t = timeElapsed / fadeDuration;

            t = animCurve.Evaluate(t);

            a.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        a.alpha = endAlpha;
        if (endAlpha > 0.5) { a.interactable = true;}  
        if (endAlpha < 0.5) { a.gameObject.SetActive(false); }
    }

    //PAST THIS POINT YOU WILL FIND NOTHING BUT ANTHONYS STUPID BAD CODE THIS DUDES SUCH AN IDIOT - anthony

    [SerializeField] Slider mouseSense;
    PlayerController pc;

    void Start()
    {
        mouseSense.value = PlayerPrefs.GetFloat("Sensitivity", 400);
        mouseSense.transform.Find("InputField (TMP)").GetComponent<TMP_InputField>().text = Mathf.Round(PlayerPrefs.GetFloat("Sensitivity", 400)).ToString();

        pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            pc.mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 400);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void UpdateMouseSensitivity()
    {
        PlayerPrefs.SetFloat("Sensitivity", mouseSense.value);
        PlayerPrefs.Save();

        if (pc != null)
        {
            pc.mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }
    }

    public void GoToScene(string scene)
    {
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene(scene);
    }

    public void DataCollectionOptIn(CanvasGroup a)
    {
        PlayerPrefs.SetInt("EnableDataCollection", 1);
        PlayerPrefs.Save();
        DataCollection.Instance.StartCollection();
        TransitionTo(a);
    }
}
