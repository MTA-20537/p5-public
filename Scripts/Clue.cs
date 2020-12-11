using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Clue : MonoBehaviour
{
    public AudioManager audioManager;
    public bool hasBeenFound;           // whether or not the clue has been found yet
    public bool isUseful;               // whether the clue is useful of useless
    public string identifier;           // the name string to show on the clue button in the codex
    private Interactable interactable;  // the "Interactable" component on the associated GameObject
    Vector3 origin;
    void Start()
    {
        origin = transform.position;
        // Start is called before the first frame update
        interactable = gameObject.GetComponent<Interactable>();
    }

    void Update()
    {
        // if the Clue is being held by the player's hand, and it has not been found yet, it is now discovered
        if (interactable.attachedToHand != null && !hasBeenFound) {
            hasBeenFound = true;
            this.GetComponent<Interactable>().attachedToHand.TriggerHapticPulse(UInt16.MaxValue);

            audioManager.playSound("gameVO - "+gameObject.name);

            // TODO: more advanced logic for when the player has examined the clue sufficiently...
        }
        if(transform.position.y< -1)
        {
            transform.position = origin;
        }
    }
}
