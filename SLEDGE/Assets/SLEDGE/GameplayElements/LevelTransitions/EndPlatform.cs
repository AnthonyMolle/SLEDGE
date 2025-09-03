using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPlatform : MonoBehaviour
{
    // Scene we move to after the level complete screen
    public string NextScene = "MainMenu";

    // Screen that shows the player score/time
    public GameObject LevelCompleteScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    // As soon as we hit and trigger the platform, we stop the timer, but delay a few seconds to let the player launch through the air before showing the level complete screen
    public void triggerPlatform()
    {
        GameObject.Find("SpeedRunTimer").GetComponent<Speed_Run_Timer>().timer_running = false;
        StartCoroutine(DelaySequence(2.0f));
    }

    IEnumerator DelaySequence(float delay)
    {
        yield return new WaitForSeconds(delay);

        beginEndSequence();
    }

    // Pause the game world by setting time scale to 0, then start the animation for the level complete screen
    public void beginEndSequence()
    {
        Time.timeScale = 0;

        if (LevelCompleteScreen != null && LevelCompleteScreen.GetComponent<LevelCompleteScreen>() != null)
        {
            LevelCompleteScreen.GetComponent<CanvasGroup>().alpha = 1;
            LevelCompleteScreen.GetComponent<LevelCompleteScreen>().StartAnimation();
        }
    }

    // Called after we exit the level complete screen
    public void GoToScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(NextScene, LoadSceneMode.Single);
    }
}
