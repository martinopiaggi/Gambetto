using System;
using Gambetto.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Gambetto.Scripts.UI
{
    public class SlidersAndToggle : MonoBehaviour
    {
        public Slider musicSlider,
            sfxSlider;

        public Toggle unlockAllLevelsToggle;
        public Toggle disableQuotesToggle;
        public Toggle enableHighlatedSquaresToggle;

        private void Start()
        {
            musicSlider.value = AudioManager.Instance.music;
            sfxSlider.value = AudioManager.Instance.sfx;
            if (unlockAllLevelsToggle)
                unlockAllLevelsToggle.isOn = GameManager.Instance.AllLevelsUnlocked;
            if (disableQuotesToggle)
                disableQuotesToggle.isOn = GameManager.Instance.DisableQuotes;
            if (enableHighlatedSquaresToggle)
                enableHighlatedSquaresToggle.isOn = GameManager.Instance.HighLightedSquaresActive;
        }

        public void MusicVolume()
        {
            AudioManager.Instance.EditMusicVolume(musicSlider.value);
        }

        public void SfxVolume()
        {
            AudioManager.Instance.EditSfxVolume(sfxSlider.value);
        }

        public void UnlockAllLevels()
        {
            GameManager.Instance.AllLevelsUnlocked = unlockAllLevelsToggle.isOn;
        }

        public void DisableQuotes()
        {
            GameManager.Instance.DisableQuotes = disableQuotesToggle.isOn;
        }

        public void HighLightedSquaresActive()
        {
            GameManager.Instance.HighLightedSquaresActive = enableHighlatedSquaresToggle.isOn;
        }

        public void HandleQualityInput(int val)
        {
            // convert value to actual QualityLevel index
            val = val switch
            {
                0 => 2, // high
                1 => 1, // low
                _ => 2
            };
            GameManager.Instance.Quality = val;
        }
    }
}