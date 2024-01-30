using System;
using System.Collections;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Range(0, 1)]
        public float music = 0.5f,
            sfx = 0.5f;

        [Header("---- Audio Source ----")]
        [SerializeField]
        private AudioSource musicSource;

        [SerializeField]
        private AudioSource sfxSource;

        [Header("---- Audio Clip ----")]
        [Header("--------Background Clips----------")]
        public AudioClip menuBackground;

        [Header("--------SFX Clips----------")]
        public AudioClip pawnMovement;
        public AudioClip clockTick;
        public AudioClip chosenMove;
        public AudioClip deathByCollision;
        public AudioClip deathByFall;
        public AudioClip levelFinished;
        public AudioClip powerUp;
        public AudioClip enemyAlerted;

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

            // play background music in menu and to load player volumes previously set
            try
            {
                music = PlayerPrefs.GetFloat("MusicVolume", 0.25f);
                sfx = PlayerPrefs.GetFloat("SFXVolume", 0.25f);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
            //set values
            EditMusicVolume(music);
            EditSfxVolume(sfx);
        }

        //method used to play every sfx
        public void PlaySfx(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        //method used to change background music
        public void PlayBackground(AudioClip clip)
        {
            PlayWithFadeIn(clip, 4f);
        }

        //method that start music with a fade in
        private void PlayWithFadeIn(AudioClip clip, float duration)
        {
            StartCoroutine(FadeOutFadeIn(clip, duration));
        }

        //method that start music with a fade in
        private IEnumerator FadeOutFadeIn(AudioClip clip, float duration)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.volume = 0;
            musicSource.Play();

            var timeElapsed = 0f;
            while (musicSource.volume < music)
            {
                musicSource.volume = Mathf.Lerp(0, 1, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        // private IEnumerator PlayWithFadeInCoroutine(AudioClip clip, float duration)
        // {
        //     musicSource.Stop();
        //     musicSource.clip = clip;
        //     musicSource.volume = 0;
        //     musicSource.Play();
        //     var elapsedTime = 0f;
        //     while (elapsedTime < duration)
        //     {
        //         elapsedTime += Time.deltaTime;
        //         musicSource.volume = Mathf.Lerp(0, music, elapsedTime / duration);
        //         yield return null;
        //     }
        // }

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
