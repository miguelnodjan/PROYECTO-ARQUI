using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_volumen : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider; 
    public float sliderValue;
    public Image imageMute;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        AudioListener.volume = slider.value;
        IsMute();
    }

    public void ChangeSliderValue(float value)
    {
        sliderValue = value;
        PlayerPrefs.SetFloat("volumenAudio", sliderValue);
        AudioListener.volume = slider.value;
        IsMute();
    }

    public void IsMute()
    {
        imageMute.enabled = slider.value == 0;
    }
}
