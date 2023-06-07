using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour{
    [SerializeField] private AudioClip rocketEffect;
    private AudioSource audioSource;
    private float musicVolume;
    private float effectVolume;
    private int effectsRunning = 0;
    
    void Awake(){
        // Check whether music and effects should be played
        bool music = PlayerPrefs.GetInt("music", 1) == 1;
        bool effects = PlayerPrefs.GetInt("soundEffects", 1) == 1;

        // Retrieve the music volume, if it should be played
        // Set the music volume to 0, if it shouldn't
        if(music){
            musicVolume = PlayerPrefs.GetFloat("musicVolume", 1);
        }else{
            musicVolume = 0;
        }

        // Do the same for the effects
        if(effects){
            effectVolume = PlayerPrefs.GetFloat("soundEffectVolume", 1);
        }else{
            effectVolume = 0;
        }

        // Retrieve the audio source, and set it's volume to the music volume
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = musicVolume;
    }

    IEnumerator PlaySoundEffect(AudioClip clip){
        // Set the volume to 1, if the music volume is lower than the effect volume
        if(audioSource.volume < effectVolume){
            audioSource.volume = 1;
        }

        // Play the sound effect
        audioSource.PlayOneShot(clip, effectVolume);

        // Increment the number of effects playing
        effectsRunning++;

        // Wait until the effect is done playing
        yield return new WaitForSeconds(clip.length);

        // Decrement the number of effects playing
        effectsRunning--;

        // Set the volume back to the music volume, if there are no effects left playing
        if(effectsRunning == 0){
            audioSource.volume = musicVolume;
        }
    }

    // Start playing the missile sound effect
    public void PlayMissileEffect() => StartCoroutine(PlaySoundEffect(rocketEffect));
}
