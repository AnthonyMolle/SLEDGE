using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{
    public string NextScene = "Kat";

    public GameObject LevelCompleteScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        // Make portal invisible
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Time.timeScale = 0;
            LevelCompleteScreen.SetActive(true);

            if (LevelCompleteScreen != null && LevelCompleteScreen.GetComponent<LevelCompleteScreen>() != null)
            {
                LevelCompleteScreen.GetComponent<LevelCompleteScreen>().StartAnimation();
            }
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
