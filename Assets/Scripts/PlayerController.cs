using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour{
    [SerializeField] private float force = 1;
    [SerializeField] private float rotationSpeed = 90;
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private LevelManager manager;
    [SerializeField] private SoundManager sound;
    private Rigidbody2D playerRigidbody;
    private float movement;
    private float rotation;
    private bool paused = false;
    private Vector2 velocity;

    void Awake(){
        // Cache a reference to the rigidbody of the player
        playerRigidbody = GetComponent<Rigidbody2D>();
        manager.PauseEvent.AddListener(Pause);
        manager.ResumeEvent.AddListener(Resume);
    }

    void Update(){
        /// We don't want the player to rotate, if the game is paused
        if(paused){
            return;
        }

        // Rotate the player, based on input
        transform.Rotate(rotation * rotationSpeed * Vector3.back * Time.deltaTime);
    }

    void FixedUpdate(){
        // We don't want to move the player, if the game is paused
        if(paused){
            return;
        }

        // Apply forwards force on the player, based on input
        Vector3 forceVector = transform.TransformDirection(movement * Vector3.up * force);
        playerRigidbody.AddForce(forceVector, ForceMode2D.Force);
    }

    void OnBecameInvisible(){
        // Make the player appear on the other side of the screen, if they left the screen
        if(transform.position.x < -manager.xRange - transform.localScale.x){
            transform.position += (manager.xRange + transform.localScale.x) * 2 * Vector3.right;
        }else if(transform.position.x > manager.xRange + transform.localScale.x){
            transform.position += (manager.xRange + transform.localScale.x) * 2 * Vector3.left;
        }
        if(transform.position.y < -manager.yRange - transform.localScale.y){
            transform.position += (manager.yRange + transform.localScale.y) * 2 * Vector3.up;
        }else if(transform.position.y > manager.yRange + transform.localScale.y){
            transform.position += (manager.yRange + transform.localScale.y) * 2 * Vector3.down;
        }
    }

    // Store the forward/backward movement input as a float
    void OnMove(InputValue value) => movement = value.Get<float>();

    // Store the rotation input as a float
    void OnRotate(InputValue value) => rotation = value.Get<float>();

    void OnShoot(InputValue value){
        // Spawn a missile in front of the player and start a rocket sound effect, if the player presses the shoot button
        Vector3 spawnPosition = transform.position + transform.TransformDirection(Vector3.up);
        Instantiate(missilePrefab, spawnPosition, transform.rotation).Init(manager);
        sound.PlayMissileEffect();
    }

    void Pause(){
        // Store the velocity of the player and stop it from moving, if the game is paused
        paused = true;
        velocity = playerRigidbody.velocity;
        playerRigidbody.bodyType = RigidbodyType2D.Static;
    }

    void Resume(){
        // Allow the player to move again and restore the velocity, if the game is resumed
        paused = false;
        playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
        playerRigidbody.velocity = velocity;
    }

    public void GameOver(){
        // Notify the manager that the game is over, and destroy the player, if the game is over
        manager.GameOver();
        Destroy(gameObject);
    }
}
