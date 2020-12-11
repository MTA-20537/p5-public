using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The class responsible for playing the infamous "Shannon"-line
 */
public class PlayAudio : MonoBehaviour
{
    public AudioManager audioManager;   // the associated AudioManager, the means bu which the sound is played
    public bool hasPlayed = false;      // whether or not the sound has already played
    public Transform cam;               // the player camera
    public float distThresh;            // the distance threshold for when the sound should be played

    /**
     * update is called once per frame
     */
    private void Update()
    {
        // play the sound if the player is within the threshold distance and the sound has not already played
        if(Vector3.Distance(transform.position,cam.position) < distThresh && !hasPlayed)
        {
            // play the sound file named "gameVO - Bodybag" and make sure it is not played more than once
            audioManager.playSound("gameVO - Bodybag");
            hasPlayed = true;
        }
    }
    
        
    
}
