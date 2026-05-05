using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : Singleton<AudioManager>
{
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1f;
    [Range(0, 1)]
    public float musicVolume = 1f;
    [Range(0, 1)]
    public float sfxVolume = 1f;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    [Header("Bank Loader")]
    public StudioBankLoader bankLoader;

    [Header("Music")]
    public EventReference MainMenuMusic;
    public EventReference LevelMusic;
    private EventInstance menuMusicInstance;
    private EventInstance lvlMusicInstance;

    [Header("Player SFX")]
    public EventReference PlayerFootstepTile;
    public EventReference PlayerHammerHit;
    public EventReference PlayerHammerWhiff;
    public EventReference PlayerLandOnGround;

    [Header("Gameplay SFX")]
    public EventReference CheckpointRespawn;
    public EventReference CheckpointActivate;
    public EventReference PhaseWallImpact;
    public EventReference PhaseIdle;
    public EventReference PhasePass;
    public EventReference PowerupExplosive;
    public EventReference PowerupPickUp;
    public EventReference SwitchActivate;
    public EventReference DeathScreen;
    public EventReference LevelComplete;
    public EventReference ShooterCharge;
    public EventReference FlyerCharge;
    public EventReference FlyerAttack;

    protected override void Awake()
    {
        base.Awake();
        bankLoader.Load();
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "MainMenu")
        {
            menuMusicInstance = CreateEventInstance(MainMenuMusic);
            menuMusicInstance.start();
            //musicSource.clip = mainMenu;
            //musicSource.Play();
        }
        else if(sceneName == "Level1" || sceneName == "Level2")
        {
            lvlMusicInstance = CreateEventInstance(LevelMusic);
            lvlMusicInstance.start();
            //musicSource.clip = background;
            //musicSource.Play();
        }

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/MUSIC");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    // Code for probably keeping the music playing between scenes?\
    public static string sceneName;
    public static string prevScene = "";

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
        /*
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if(sceneName == "MainMenu" && prevScene != "MainMenu")
        {
            menuMusicInstance = CreateEventInstance(MainMenuMusic);
            menuMusicInstance.start();
            prevScene = "MainMenu";
            Debug.Log("main menu scene");
            //musicSource.clip = mainMenu;
            //musicSource.Play();
        }
        */
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

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventRef, GameObject emitterGameObj)
    {
        StudioEventEmitter emitter = emitterGameObj.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventRef;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void Cleanup()
    {
        //stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        //emitters die when change scene
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    public bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}
