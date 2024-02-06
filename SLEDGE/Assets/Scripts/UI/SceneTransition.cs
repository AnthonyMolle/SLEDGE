using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{ 
    public Animator transition;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = sceneIndex + 1;

        if (SceneManager.sceneCount >= nextSceneIndex)
        {
            StartCoroutine(LoadLevel(nextSceneIndex));
        }
        else
        {
            StartCoroutine(LoadLevel(sceneIndex));
        }
    }

    IEnumerator LoadLevel(int nextSceneIndex)
    {

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nextSceneIndex);

        transition.SetTrigger("Clear");

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
