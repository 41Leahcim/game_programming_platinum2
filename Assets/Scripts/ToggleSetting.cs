using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSetting : MonoBehaviour{
    [SerializeField] private string settingName;
    private Toggle toggle;

    void Awake(){
        // Cache a reference to the toggle, and restore it's value
        toggle = GetComponent<Toggle>();
        toggle.isOn = PlayerPrefs.GetInt(settingName, 1) == 1;
    }

    // Store the value of the toggle
    public void Save() => PlayerPrefs.SetInt(settingName, toggle.isOn? 1 : 0);
}
