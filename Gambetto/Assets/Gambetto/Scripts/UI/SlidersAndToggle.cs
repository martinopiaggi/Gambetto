using POLIMIGameCollective.Scripts.Movement.TurnBased;
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
            unlockAllLevelsToggle.isOn = GameManager.Instance.AllLevelsUnlocked;
            disableQuotesToggle.isOn = GameManager.Instance.DisableQuotes;
            
            
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
            PlayerPrefs.SetInt("AllLevelsUnlocked", GameManager.Instance.AllLevelsUnlocked ? 1 : 0);
        }

        public void DisableQuotes()
        {
            GameManager.Instance.DisableQuotes = disableQuotesToggle.isOn;
            PlayerPrefs.SetInt("DisableQuotes", GameManager.Instance.DisableQuotes ? 1 : 0);
        }
        
        public void HighLightedSquaresActive()
        {
            GameManager.Instance.HighLightedSquaresActive = enableHighlatedSquaresToggle.isOn;
            PlayerPrefs.SetInt("HighLightedSquaresActive", GameManager.Instance.HighLightedSquaresActive ? 1 : 0);
        }
    }
}
