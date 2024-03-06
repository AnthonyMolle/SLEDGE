using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Manager : MonoBehaviour
{
    public AudioMixer mixer;

    public Shared_Slider_State master_slider;
    public Shared_Slider_State sfx_slider;
    public Shared_Slider_State music_slider;

    private void Start()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 50);
        float music = PlayerPrefs.GetFloat("MusicVolume", 50);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 50);

        mixer.SetFloat("MasterVolume", Mathf.Log10(master / master_slider.max) * 20);
        mixer.SetFloat("MusicVolume", Mathf.Log10(music / music_slider.max) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(sfx / sfx_slider.max) * 20);

        master_slider.UpdateValue(master);
        sfx_slider.UpdateValue(sfx);
        music_slider.UpdateValue(music);
    }

    public void UpdateMaster()
    { 

        // Ensure mixes dont break!
        float master = Mathf.Max(master_slider.value, 0.001f) / master_slider.max;

        mixer.SetFloat("MasterVolume", Mathf.Log10(master) * 20);

        PlayerPrefs.SetFloat("MasterVolume", Mathf.Max(master_slider.value, 0.001f));
    }

    public void UpdateMusic()
    {

        // Ensure mixes dont break!
        float music = Mathf.Max(music_slider.value, 0.001f) / music_slider.max;

        mixer.SetFloat("MusicVolume", Mathf.Log10(music) * 20);
 
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Max(music_slider.value, 0.001f));
    }

    public void UpdateSFX()
    {

        // Ensure mixes dont break!
        float sfx = Mathf.Max(sfx_slider.value, 0.001f) / sfx_slider.max;

        mixer.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20);

        PlayerPrefs.SetFloat("SFXVolume", Mathf.Max(sfx_slider.value, 0.001f));
    }
}
