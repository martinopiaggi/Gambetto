using UnityEngine;
using UnityEngine.UI;

namespace Gambetto.Scripts.UI
{
    public class SlidersAndToggle : MonoBehaviour
    {
        public Slider musicSlider,
            sfxSlider;
        public Toggle unlockAllLevelsToggle;

        private void Start()
        {
            musicSlider.value = AudioManager.Instance.music;
            sfxSlider.value = AudioManager.Instance.sfx;
            unlockAllLevelsToggle.isOn = PlayerPrefs.GetInt("AllLevelsUnlocked", 0) == 1;
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
    }
}
