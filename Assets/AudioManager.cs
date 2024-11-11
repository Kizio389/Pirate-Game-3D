using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource; 
    public Slider globalSlider;     
    public Slider musicSlider;     

    private float globalVolume = 1f;
    private float musicVolume = 1f;

    void Start()
    {
        audioSource.volume = globalVolume * musicVolume;
        globalSlider.onValueChanged.AddListener(SetGlobalVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    void SetGlobalVolume(float volume)
    {
        globalVolume = volume;
        UpdateAudioVolume();
    }

    void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateAudioVolume();
    }

    void UpdateAudioVolume()
    {
        audioSource.volume = globalVolume * musicVolume;
    }
}
