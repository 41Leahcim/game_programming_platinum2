using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour{
    public UnityEvent PauseEvent;
    public UnityEvent ResumeEvent;
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private float minimumSpawnDistance = 1;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas gameOverMenu;
    [SerializeField] private Score score;
    private List<Asteroid> asteroids = new List<Asteroid>();
    public float xRange{get; private set;}
    public float yRange{get; private set;}
    private int level = 1;
    
    void CalculateScreenSize(){
        // Store the size of the screen in unity units
        yRange = Camera.main.orthographicSize;
        xRange = Camera.main.aspect * yRange;
        yRange -= asteroidPrefab.transform.localScale.y / 2;
        xRange -= asteroidPrefab.transform.localScale.x / 2;
    }

    void Awake(){
        // Calculate and store the screen size
        CalculateScreenSize();

        // Start the level, by spawning asteroids
        StartLevel();
    }

    void StartLevel(){
        // Spawn the asteroids on a preset distance from other asteroids and the player
        for(int i = 0;i < level;i++){
            Vector3  spawnPosition = new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange));
            if(IsSpawnPositionValid(spawnPosition)){
                // Spawn the asteroid
                Quaternion rotation = asteroidPrefab.transform.rotation;
                asteroids.Add(Instantiate(asteroidPrefab, spawnPosition, rotation).Init(this));
            }else{
                i--;
            }
        }
    }

    bool IsSpawnPositionValid(Vector3 spawnPosition){
        // Check whether the asteroid is too close to the player, return false if it is
        if(Vector3.Distance(spawnPosition, player.transform.position) < minimumSpawnDistance){
            return false;
        }

        // Check whether the asteroid is too close to any other asteroid, return false if it is
        foreach(Asteroid asteroid in asteroids){
            if(Vector3.Distance(spawnPosition, asteroid.transform.position) < minimumSpawnDistance){
                return false;
            }
        }

        // Return true, as it is far enough from other objects
        return true;
    }

    public void AsteroidDestroyed(Asteroid destroyed){
        // Increment and update the score
        score.Add(1);

        // Remove the asteroid from the list
        asteroids.Remove(destroyed);

        // Start the next level, if the player destroyed all asteroids
        if(asteroids.Count == 0){
            level++;
            StartLevel();
        }
    }

    public void AsteroidSplit(Asteroid child){
        // Increment the score
        score.Add(1);

        // Add the child to the list
        asteroids.Add(child);
    }

    public void GameOver(){
        // Display the game over menu and remove 1 from the score
        gameOverMenu.enabled = true;
        PauseEvent.RemoveAllListeners();
        ResumeEvent.RemoveAllListeners();
        score.Add(-1);
    }
    
    private void OnPause(){
        // Display the pause menu and pause every moving object
        pauseMenu.enabled = true;
        PauseEvent.Invoke();
    }

    // Resume every moving object
    public void Resume() => ResumeEvent.Invoke();
}
