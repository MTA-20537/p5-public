using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioManager audioManager;
    public bool hasPlayed = false;
    public Transform cam;
    public float distThresh;
    private void Update()
    {
        if(Vector3.Distance(transform.position,cam.position) < distThresh && !hasPlayed)
        {
            print("TRIGGERED");
            audioManager.playSound("gameVO - Bodybag");
            hasPlayed = true;
        }
    }
    
        
    
}
