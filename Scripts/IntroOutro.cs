using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The class responsible for managing the intro- and outro sequences of the game
 */
public class IntroOutro : MonoBehaviour
{
    public Animator animator;               // the "fade-to-black" animator
    public AudioManager audioManager;       // the audio manager object
    public string anim;                     // which animation to do
    private GameObject[] hands;             // the hands of the player
    public GameObject teleporter;           // the player teleporter
    public LoggingManager loggingManager;   // the logging manager object

    // Start is called before the first frame update
    void Start()
    {
        // reference hands and play into on startup
        hands = GameObject.FindGameObjectsWithTag("Hand");
        playIntro();
    }

    public void playIntro()
    {
        allowPlayerTeleportation(false);            // disallow player teleportation during intro sequence
        anim = "FIn";                               // play the "fade-in" animation next
        audioManager.playSound("gameVO - intro");   // play the introduction voice-over sequence
        Invoke("playSimple", 8.00f);                // update the state of animations and player teleportation
    }

    public void playOutro()
    {
        anim = "FOut";                              // play the "fade-out" animation next
        updateState(false);                         // update the state of animations and player teleportation
        audioManager.playSound("gameVO - outro");   // play the outro voice-over sequence
        Invoke("playOutro2", 22.5f);                // player the final voice over sequence after the first one has finished
    }

    /**
     * invoke the "updateState" method with no provided parameters
     * this is only necessary because "Invoke" is weird
     */
    public void playSimple()
    {
        updateState(); // no param = true
    }

    public void updateState(bool state = true)
    {
        animator.SetTrigger(anim);          // play the queued animation
        allowPlayerTeleportation(state);    // allow or disallow player teleportation according to provided state
    }

    /**
     * plays the final voice-over sequence
     */
    public void playOutro2()
    {
        audioManager.playSound("gameVO - Resiting");
    }

    /**
     * toggle the player's ability to teleport with porvided state
     */
    private void allowPlayerTeleportation(bool state)
    {
        print("ALLOW LOGGING? " + state);
        teleporter.SetActive(state);            // toggle the player's ability to teleport
        loggingManager.allowLogging = state;    // make sure to disable teleportation logging when player teleportation is disabled
    }
}