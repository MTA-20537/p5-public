using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

/**
 * The class representing a Clue
 */
public class Clue : MonoBehaviour
{
    public AudioManager audioManager;   // the audio manager
    public bool hasBeenFound;           // whether or not the clue has been found yet
    public bool isUseful;               // whether the clue is useful of useless
    public string identifier;           // the name string to show on the clue button in the codex
    private Interactable interactable;  // the "Interactable" component on the associated GameObject
    Vector3 origin;                     // the origin point of the Clue, used to reset it's position if it is displaced

    /**
     * start is called before the first frame update
     */
    void Start()
    {
        origin = transform.position;
        // Start is called before the first frame update
        interactable = gameObject.GetComponent<Interactable>();
    }

    /** 
     * update is called once per frame
     */
    void Update()
    {
        // if the Clue is being held by the player's hand, and it has not been found yet, it is now discovered
        if (interactable.attachedToHand != null && !hasBeenFound)
        {
            hasBeenFound = true;
            this.GetComponent<Interactable>().attachedToHand.TriggerHapticPulse(UInt16.MaxValue); // trigger a haptic sensation in the player controllers
            audioManager.playSound("gameVO - "+gameObject.name); // play voice-over sequence associated with Clue
        }

        // reset Clue's position if it has been displaced through the floor
        if (transform.position.y < -1) transform.position = origin;
    }
}
