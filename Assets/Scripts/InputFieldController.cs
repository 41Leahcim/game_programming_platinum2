using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class InputFieldController : MonoBehaviour{
    [SerializeField] private string settingName;
    private TMPro.TMP_InputField inputField;
    private string originalValue;

    void Awake(){
        // Retrieve the input field
        inputField = GetComponent<TMPro.TMP_InputField>();

        // Retrieve and set the player name
        originalValue = PlayerPrefs.GetString(settingName, "");
        inputField.text = originalValue;
    }

    public void SaveName(){
        // Save the player name, if it changed
        if(inputField.text != originalValue){
            PlayerPrefs.SetString(settingName, inputField.text);
        }
    }
}
