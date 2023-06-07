using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class Score : MonoBehaviour{
    [SerializeField] private string scoreFile;
    private TMPro.TMP_Text scoreText;
    private int score = 0;

    void Awake(){
        // Cache a reference to the score text, and set the initial score text
        scoreText = GetComponent<TMPro.TMP_Text>();
        scoreText.text = "Score: " + score;
    }

    public void Add(int value){
        // Add to the score, and update the text
        score += value;
        scoreText.text = "Score: " + score;
    }

    public async void Save(){
        // Retrieve the name of the player, use Player as default
        string playerName = PlayerPrefs.GetString("name", "Player");

        // If the score file doesn't exist, create it and use the score and players name as content
        // If it does, just append the players name and content at the end of the file
        if(File.Exists(scoreFile)){
            await File.AppendAllTextAsync(scoreFile, string.Format($"\n{score},{playerName}"));
        }else{
            File.Create(scoreFile);
            await File.WriteAllTextAsync(scoreFile, string.Format($"{score},{playerName}"));
        }
    }
}
