using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using FMODUnity;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Player SFX")]
    public EventReference PlayerFootstepTile;
    public EventReference PlayerHammerHit;
    public EventReference PlayerHammerWhiff;
    public EventReference PlayerLandOnGround;

    [Header("Gameplay SFX")]
    public EventReference CheckpointRespawn;
    public EventReference PowerupPickUp;
    public EventReference SwitchActivate;

    /*
    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if(sceneName == "Jonah")
        {
            musicSource.clip = mainMenu;
            musicSource.Play();
        }
        else if(sceneName == "Anthony Enemy Implementation" || sceneName == "Easy Level ART" || sceneName == "Mid Level ART"|| sceneName == "Josh")
        {
            musicSource.clip = background;
            musicSource.Play();
        }
    }
    */

    // Code for probably keeping the music playing between scenes?\
    public static string sceneName;
    public static string prevScene = "";

    private void Update()
    {
        // Scene currentScene = SceneManager.GetActiveScene();
        // string sceneName = currentScene.name;
        // if(sceneName == "Jonah" && prevScene != "Jonah")
        // {
        //     musicSource.clip = mainMenu;
        //     musicSource.Play();
        //     prevScene = "Jonah";
        // }
        // else if(sceneName == "Anthony Enemy Implementation" || sceneName == "Easy Level ART" || sceneName == "Mid Level ART" || sceneName == "EvanLevel3")
        // {
        //     if(musicSource.clip != background)
        //     {
        //         musicSource.Stop();
        //         musicSource.clip = background;
        //         musicSource.Play();
        //         prevScene = "";
        //     }
        // }
    }

    public void PlayOneShotSFX2D(EventReference sfxEvent)
    {
        RuntimeManager.PlayOneShot(sfxEvent, transform.position);
    }

    public void PlayOneShotSFX3D(EventReference sfxEvent, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sfxEvent, position);
    }
}
