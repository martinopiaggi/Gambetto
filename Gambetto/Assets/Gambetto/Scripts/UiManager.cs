using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(musicSlider.value);
    }
    
    public void SfxVolume()
    {
        AudioManager.Instance.SfxVolume(sfxSlider.value);
    }
}
