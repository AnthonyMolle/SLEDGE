using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

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
}
