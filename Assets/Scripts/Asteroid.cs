using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour{
    [SerializeField] private float minimumScale = 1f;
    [SerializeField] private float force = 1;
    private Rigidbody2D asteroidRigidBody;
    private LevelManager manager = null;
    private Vector2 velocity;
    private bool paused;
    
    void Awake(){
        // Cache a reference to the Rigidbody2D component of the asteroid
        asteroidRigidBody = GetComponent<Rigidbody2D>();

        // Push the asteroid in a random direction
        Vector3 forceDirection = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0).normalized;
        asteroidRigidBody.AddForce(forceDirection * force, ForceMode2D.Impulse);
    }

    void FixedUpdate(){
        // We don't have to keep the asteroid inbounds, if the game is paused as the asteroid can't move.
        if(paused){
            return;
        }

        // Push the Asteroid away from the upper or lower border, if it's going to cross it
        if(transform.position.y < -manager.yRange){
            Vector3 forceDirection = new Vector3(0, Mathf.Abs(asteroidRigidBody.velocity.y * 2), 0);
            asteroidRigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            transform.TransformDirection(Vector3.up * (manager.yRange - transform.position.y));
        }else if(transform.position.y > manager.yRange){
            Vector3 forceDirection = new Vector3(0, -Mathf.Abs(asteroidRigidBody.velocity.y * 2), 0);
            asteroidRigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            transform.TransformDirection(Vector3.down * (manager.yRange - transform.position.y));
        }

        // Push the Asteroid away from the left or right border, if it's going to cross it
        if(transform.position.x < -manager.xRange){
            Vector3 forceDirection = new Vector3(Mathf.Abs(asteroidRigidBody.velocity.x * 2), 0, 0);
            asteroidRigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            transform.TransformDirection(Vector3.left * (manager.xRange - transform.position.x));
        }else if(transform.position.x > manager.xRange){
            Vector3 forceDirection = new Vector3(-Mathf.Abs(asteroidRigidBody.velocity.x * 2), 0, 0);
            asteroidRigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            transform.TransformDirection(Vector3.right * (manager.xRange - transform.position.x));
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        // Try to get the missile or player component of the object the asteroid collided with
        Missile missile = collision.gameObject.GetComponent<Missile>();
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        // If it collided with a missile, destroy the missile and split or destroy the asteroid
        // If it collided with the player instead, the game is over
        if(missile != null){
            missile.Destroy();
            SplitOrDestroy();
        }else if(player != null){
            player.GameOver();
            SplitOrDestroy();
        }
    }

    void SplitOrDestroy(){
        // If the asteroid is larger than the minimum scale, split it
        // Notify the manager that the asteroid has been destroyed, if not
        if(transform.localScale.magnitude > minimumScale){
            Split();
        }else{
            manager.AsteroidDestroyed(this);
            manager.PauseEvent.RemoveListener(Pause);
            manager.ResumeEvent.RemoveListener(Resume);
            Destroy(gameObject);
        }
    }

    void Split(){
        // Calculate the spawn position of the clone
        Vector3 spawnPosition = transform.position + transform.TransformDirection(Vector3.right);

        // Move this asteroid and halve it in size
        transform.Translate(transform.TransformDirection(Vector3.left));
        transform.localScale /= 2;

        // Clone the asteroid with the pre-calculated spawn position
        Asteroid clone = Instantiate(this, spawnPosition, transform.rotation).Init(manager);

        // Notify the manager, that the asteroid split
        manager.AsteroidSplit(clone);
    }

    public Asteroid Init(LevelManager levelManager){
        // Set the manager, if it hasn't already
        if(manager == null){
            manager = levelManager;

            // Start listening for pause and resume events
            manager.PauseEvent.AddListener(Pause);
            manager.ResumeEvent.AddListener(Resume);
        }

        // Return a reference to this asteroid
        return this;
    }

    void Pause(){
        // Store the current velocity and stop the asteroid
        paused = true;
        velocity = asteroidRigidBody.velocity;
        asteroidRigidBody.bodyType = RigidbodyType2D.Static;
    }

    void Resume(){
        // Allow the asteroid to move again, and restore it's velocity
        paused = false;
        asteroidRigidBody.bodyType = RigidbodyType2D.Dynamic;
        asteroidRigidBody.velocity = velocity;
    }
}
