using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPlatform : MonoBehaviour
{
    public string NextScene = "Jonah";

    public GameObject LevelCompleteScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void triggerPlatform()
    {
        StartCoroutine(DelaySequence(2.0f));
    }

    IEnumerator DelaySequence(float delay)
    {
        yield return new WaitForSeconds(delay);

        beginEndSequence();
    }

    public void beginEndSequence()
    {
        Time.timeScale = 0;

        if (LevelCompleteScreen != null && LevelCompleteScreen.GetComponent<LevelCompleteScreen>() != null)
        {
            LevelCompleteScreen.GetComponent<CanvasGroup>().alpha = 1;
            LevelCompleteScreen.GetComponent<LevelCompleteScreen>().StartAnimation();
        }
    }

    public void GoToScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(NextScene, LoadSceneMode.Single);
    }
}
