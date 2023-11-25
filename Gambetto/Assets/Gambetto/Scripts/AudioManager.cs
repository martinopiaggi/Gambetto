using System;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public float music = 0.5f, sfx = 0.5f;
    
    [Header("---- Audio Source ----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---- Audio Clip ----")]
    
    [Header("--------Background Clips----------")]
    public AudioClip menuBackground;
    
    [Header("--------SFX Clips----------")]
    public AudioClip pawnMovement;

    
    //start method to play background music in menu and to load player volumes previously set
    public void Start()
    {
        //load floats
        try
        {
            music = PlayerPrefs.GetFloat("MusicVolume");
            sfx = PlayerPrefs.GetFloat("SFXVolume");
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
        //set values
        EditMusicVolume(music);
        EditSfxVolume(sfx);
        //play background music
        musicSource.clip = menuBackground;
        musicSource.Play();
        
    }

    //awake method makes sure that AudioManager is not destroyed
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //method used to play every sfx
    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    //method used to change background music
    public void PlayBackground(AudioClip clip)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }
    
    //method used to change music volume
    public void EditMusicVolume(float volume)
    {
        music = volume;
        musicSource.volume = volume;
        //save value into PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", music);
    }
    
    //method used to change sfx volume
    public void EditSfxVolume(float volume)
    {
        sfx = volume;
        sfxSource.volume = volume;
        //save value into PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", sfx);
    }
}

}
