using UnityEngine;
using UnityEngine.UI;

namespace Gambetto.Scripts.UI
{
    public class Sliders : MonoBehaviour
    {
        public Slider musicSlider,
            sfxSlider;

        private void Start()
        {
            musicSlider.value = AudioManager.Instance.music;
            sfxSlider.value = AudioManager.Instance.sfx;
        }

        public void MusicVolume()
        {
            AudioManager.Instance.EditMusicVolume(musicSlider.value);
        }

        public void SfxVolume()
        {
            AudioManager.Instance.EditSfxVolume(sfxSlider.value);
        }
    }
}
