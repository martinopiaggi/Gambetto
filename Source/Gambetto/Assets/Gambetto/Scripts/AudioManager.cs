using System;
using System.Collections;
using Unity.VisualScripting;
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
        private AudioSource musicSourceVoicing;

        [SerializeField]
        private AudioSource sfxSource;

        [Header("---- Audio Clip ----")]
        [Header("--------Background Clips----------")]
        public AudioClip menuBackground;

        public AudioClip menuBackgroundVoicing;

        [Header("--------SFX Clips----------")]
        public AudioClip pawnMovement;
        public AudioClip clockTick;
        public AudioClip chosenMove;
        public AudioClip deathByCollision;
        public AudioClip deathByFall;
        public AudioClip levelFinished;
        public AudioClip powerUp;
        public AudioClip enemyAlerted;
        public AudioClip bombExplosion;
        public AudioClip keyUnlock;

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

        public void PlayBackground()
        {
            PlayWithFadeIn(4f);
        }

        private void PlayWithFadeIn(float duration)
        {
            StartCoroutine(BackgroundMusicCoroutine(duration));
        }

        private IEnumerator BackgroundMusicCoroutine(float duration)
        {
            musicSource.Stop();
            musicSource.clip = menuBackground;
            musicSource.volume = 0;
            musicSource.Play();
            musicSourceVoicing.Stop();
            musicSourceVoicing.clip = menuBackgroundVoicing;
            musicSourceVoicing.volume = 0;
            musicSourceVoicing.Play();
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0, music, elapsedTime / duration);
                musicSourceVoicing.volume = Mathf.Lerp(0, music, elapsedTime / duration);
                yield return null;
            }
        }

        //method used to change music volume
        public void EditMusicVolume(float volume)
        {
            music = volume;
            musicSource.volume = volume;
            musicSourceVoicing.volume = volume;
            //save value into PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", music);
        }

        Coroutine _fadeCoroutine;

        public void FadeDownVoicing(float duration)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeVoicingCoroutine(duration, true));
        }

        public void FadeUpVoicing()
        {
            const float duration = 1f;
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeVoicingCoroutine(duration, false));
        }

        private IEnumerator FadeVoicingCoroutine(float duration, bool toZero)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                musicSourceVoicing.volume = Mathf.Lerp(
                    musicSourceVoicing.volume,
                    toZero ? 0 : music,
                    elapsedTime / duration
                );
                yield return null;
            }
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
