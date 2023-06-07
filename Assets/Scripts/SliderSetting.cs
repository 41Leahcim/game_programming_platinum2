using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetting : MonoBehaviour{
    [SerializeField] private string settingName;
    private Slider slider;

    void Awake(){
        // Cache a reference to the slider, and restore it's value
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat(settingName, 1);
    }
    
    // Store the value of the slider
    public void Save() => PlayerPrefs.SetFloat(settingName, slider.value);
}
