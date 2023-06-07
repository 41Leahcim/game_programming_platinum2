using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Missile : MonoBehaviour{
    [SerializeField] private float force;
    private Rigidbody2D missileRigidBody;
    private LevelManager manager = null;
    private Vector2 velocity;

    void Awake(){
        // Cache the rigidbody, and push the missile forward
        missileRigidBody = GetComponent<Rigidbody2D>();
        Vector3 forceVector = transform.TransformDirection(Vector3.up * force);
        missileRigidBody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    void OnBecameInvisible(){
        // Destroy the missile, when it becomes invisible
        Destroy();
    }

    void Pause(){
        // Store the velocity of the missile and stop it from moving, if the game is paused
        velocity = missileRigidBody.velocity;
        missileRigidBody.bodyType = RigidbodyType2D.Static;
    }

    void Resume(){
        // Allow the missile to move again and restore the velocity, if the game is resumed
        missileRigidBody.bodyType = RigidbodyType2D.Dynamic;
        missileRigidBody.velocity = velocity;
    }

    public Missile Init(LevelManager levelManager){
        // Assign the level manager if it hasn't been assigned yet
        if(manager == null){
            manager = levelManager;
            
            // Start listening for Pause and Resume events
            manager.PauseEvent.AddListener(Pause);
            manager.ResumeEvent.AddListener(Resume);
        }
        return this;
    }

    public void Destroy(){
        // Remove pause and resume listeners, and destroy the object
        manager.PauseEvent.RemoveListener(Pause);
        manager.ResumeEvent.RemoveListener(Resume);
        Destroy(gameObject);
    }
}
