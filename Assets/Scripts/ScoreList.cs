using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class ScoreList : MonoBehaviour{
    [SerializeField] private string scoreFile;

    List<string> SortFilterScores(List<string> scores){
        for(int i = 0;i < scores.Count;i++){
            string[] segments = scores[i].Split(',');
            int score;
            if(segments.Length != 2 || !int.TryParse(segments[0], out score)){
                scores.RemoveAt(i);
                i--;
            }
        }

        // Sort the scores
        scores.Sort((s1, s2) =>
            int.Parse(s2.Split(',')[0]).CompareTo(int.Parse(s1.Split(',')[0]))
        );

        // If there are more than 10 scores, remove the 10 lower scores
        if(scores.Count > 10){
            scores.RemoveRange(10, scores.Count - 10);
        }
        
        // Return the result
        return scores;
    }

    string CreateScoreList(List<string> scores){
        // Convert the scores to a more easily readable score list
        StringBuilder result = new StringBuilder();
        foreach(string score in scores){
            string[] data = score.Split(',');
            result.AppendFormat($"\n{data[0]}, {data[1]}");
        }

        // Return the result as a string
        return result.ToString();
    }

    async void Awake(){
        // Retrieve a reference to the score list
        TMPro.TMP_Text scoreList = GetComponent<TMPro.TMP_Text>();

        // We can't make the list, if the file doesn't exist
        if(!File.Exists(scoreFile)){
            scoreList.text = "No scores yet";
            return;
        }

        // Read the scores into a list
        List<string> scores = new List<string>(await File.ReadAllLinesAsync(scoreFile));

        // Filter the score list from empty lines
        scores = SortFilterScores(scores);

        // Display the result
        scoreList.text += CreateScoreList(scores);

        // Write the smaller score list to the file
        await File.WriteAllLinesAsync(scoreFile, scores.ToArray());
    }
}
