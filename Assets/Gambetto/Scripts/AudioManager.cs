using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("---- Audio Source ----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---- Audio Clip ----")]
    
    [Header("--------Background Clips----------")]
    public AudioClip menuBackground;
    
    [Header("--------SFX Clips----------")]
    public AudioClip pawnMovement;

    
    //start method to play background music in menu
    public void Start()
    {
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
    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    
    //method used to change sfx volume
    public void SfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
