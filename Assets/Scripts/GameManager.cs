using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{
    public void LoadScene(string scene){
        // Load the scene asynchronously, to keep the game running while the scene is loaded
        SceneManager.LoadSceneAsync(scene);
    }

    public void ExitGame(){
        // Exit the game
        Application.Quit(0);
    }
}
