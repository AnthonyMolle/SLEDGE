using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("-------------- Audio Source --------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-------------- Audio Clip --------------")]
    public AudioClip background;
    public AudioClip mainMenu;
    public AudioClip hit;
    public AudioClip whiff;
    public AudioClip walk;
    public AudioClip walk2;
    public AudioClip walk3;
    public AudioClip walk4;
    public AudioClip walk5;
    public AudioClip land;

    HammerSound hammerSound;

    private void Awake()
    {
        hammerSound = GameObject.FindGameObjectWithTag("HammerSound").GetComponent<HammerSound>();
    }

    int previousChoice = 1;
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

    // Code for probably keeping the music playing between scenes?
    string prevScene = "";
    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if(sceneName == "Jonah")
        {
            musicSource.clip = mainMenu;
            musicSource.Play();
            prevScene = sceneName;
        }
        else if(sceneName == "Anthony Enemy Implementation" || sceneName == "Easy Level ART" || sceneName == "Mid Level ART")
        {
            if(prevScene != "Anthony Enemy Implementation" || prevScene != "Easy Level ART" || prevScene != "Mid Level ART")
            {
                musicSource.clip = background;
                musicSource.Play();
            }
            prevScene = sceneName;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayWalk()
    {
            
        int walkChoice = UnityEngine.Random.Range(1,6); 
        if(walkChoice == 1 && previousChoice != 1)
        {
            SFXSource.PlayOneShot(walk);
        }
        else if(walkChoice == 2 && previousChoice != 2)
        {
            SFXSource.PlayOneShot(walk2);
        }
        else if(walkChoice == 3 && previousChoice != 3)
        {
            SFXSource.PlayOneShot(walk3);
        }
        else if(walkChoice == 4 && previousChoice != 4)
        {
            SFXSource.PlayOneShot(walk4);
        }
        else if(walkChoice == 5 && previousChoice != 5)
        {
            SFXSource.PlayOneShot(walk5);
        }
        else
        {
            PlayWalk();
        }                      
        previousChoice = walkChoice;
    }

    public void playHitSound()
    {
        hammerSound.HitSoundPLEASE();
        //SFXSource.PlayOneShot(hit);
    }
   
}
