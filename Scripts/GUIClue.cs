using System;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GUIClue : MonoBehaviour
{
    public Clue clue;                           // the Clue that this GUIClue represents
    public int category;                        // the current category that this GUIClue is in
    public SteamVR_Action_Boolean gripAction;   // the current player action being performed on the GUIClue
    public bool isHolding;                      // whether or not the GUIClue is being held by the player
    public bool isInteractable;                 // whether or not the player should be able to interact with the GUIClue
    public AudioClip cluePlacedSound;           // the sound effect that plays when the GUIClue has been placed in a category
    public AudioClip buttonClickedSound;        // the sound effect that plays when the GUIClue is being "moused over" or moved between categories
    public AudioManager audioManager;           // Audio manager component for playing various voice over audio files
    private Hand collidingHand;                 // whether or not the GUIClue is colliding with the player hand
    private GameObject handCollider;            // the GameObject colliding with the GUIClue, if any

    // Start is called before the first frame update
    void Start()
    {
        category = 0;               // GUIClue starts out in the "UNASSIGNED" category
        isHolding = false;          // GUIClue starts out not being held by the player
        collidingHand = null;        // GUIClue starts out not colliding with the players hand(s)
        isInteractable = false;     // GUIClue starts out not being interactable by the player
        handCollider = null;        // since the GUIClue is not being held by the player at the start of the game, the hand collider is null
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = clue.identifier;       // set button text to associated Clue's identifier string
        this.gameObject.transform.position = new Vector3(transform.position.x, -100, transform.position.z); // GUIClue starts out being moved out of sight of the player until it's associated Clue is discovered
    }

    // Update is called once per frame
    void Update()
    {
        if (!clue.hasBeenFound) return; // don't update the GUIClue if the Clue has not been found yet (here we rely on the Codex to hide the GUIClue from the player)

        updateCategory(); // update the GUIClue category based on it's x-coordinate

        // move the GUIClue to the position of the player's hand if it has just been grabbed or the player was already holding it
        if (this.isHolding || (this.collidingHand != null && this.collidingHand.IsGrabbingWithType(GrabTypes.Grip)))
        {
            this.isHolding = true;

            // "attach" button to Hand by using the Hand's x- and y-coordinates
            float newX = this.handCollider.transform.position.x - 0.03f;
            float newY = this.handCollider.transform.position.y - 0.02f;
            //                                               ^^^^^ manual offset
            // keep the button within the x-axis bounds of the Codex dashboard
            if (newX < -0.4) newX = -0.4f;
            else if (newX > 0.4) newX = 0.4f;
            // keep the button within the y-axis bounds of the Codex dashboard
            if (newY < 1.3) newY = 1.3f;
            else if (newY > 1.8) newY = 1.8f;
            // perform the final transformation
            transform.position = new Vector3(newX, newY, transform.position.z); // TODO: fine tune offset when grabbing
        }

        // give feedback to the user when they let go of the GUIClue
        if (gripAction.stateUp)
        {
            if (this.isHolding)
            {
                // play sound and haptic feedback when GUIClue is placed in a category in the Codex
                this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.cluePlacedSound, 0.3f);
                this.handCollider.GetComponent<Hand>().TriggerHapticPulse(UInt16.MaxValue);
                if(clue.isUseful && category == 1 || !clue.isUseful && category == 2)
                {
                    audioManager.playRandomCorrectSound();
                }
                else if(clue.isUseful && category == 2 || !clue.isUseful && category == 1)
                {
                    audioManager.playRandomWrongSound();
                }
            }
            this.isHolding = false;
        }

        // change the color of the GUIClue depending on type of interaction
        ColorBlock colors = this.gameObject.GetComponent<Button>().colors;
        if (this.collidingHand)
        {
            colors.normalColor = new Color32(222, 222, 222, 255);   // slightly grey-ish white
        }
        else
        {
            colors.normalColor = new Color32(255, 255, 255, 255);   // pure white
        }
        this.gameObject.GetComponent<Button>().colors = colors;     // save color to button
    }

    private bool isHand(Collider collider)
    {
        // whether the current GameObject colliding with the GUIClue is a player hand
        Hand hand = collider.gameObject.GetComponent<Hand>();
        if (collider.gameObject.tag == "Hand" && hand != null)
        {
            return true;
        }
        return false;
    }

    private void updateCategory()
    {
        // parse GUIClue position and determine which gui category it falls under
        float xPosition = this.gameObject.transform.position.x;
        int oldCategory = this.category;

        if (xPosition < -0.16)
        {
            // clue was moved to category "USELESS"
            this.category = 2;
        }
        else if (xPosition < 0.16)
        {
            // clue was moved to category "USEFUL"
            this.category = 1;
        }
        else
        {
            // clue was moved to category "UNASSGINED"
            this.category = 0;
        }

        // if the new category if different from the previous one, it means the player moved the GUIClue to a new category
        if (this.category != oldCategory)
        {
            // give the player feedback that the GUIClue was moved
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.buttonClickedSound, 0.2f);
            this.handCollider.GetComponent<Hand>().TriggerHapticPulse(UInt16.MaxValue);
        }

        
    }

    void OnTriggerEnter(Collider collider)
    {
        // this is called whenever a collider intersects with the GUIClue
        if (this.isInteractable && isHand(collider))
        {
            // if the GUIClue is currently interactable and the collider is a hand
            this.collidingHand = collider.gameObject.GetComponent<Hand>();
            this.handCollider = collider.gameObject;

            // if the GUIClue is not being held by the player, it means the player's hand has just intersected with the GUIClue
            if (!this.isHolding)
            {
                // give the player feedback that the GUIClue was "touched"
                this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.buttonClickedSound, 0.2f);
                this.handCollider.GetComponent<Hand>().TriggerHapticPulse(UInt16.MaxValue);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        // this is called whenever a collider stops intersecting with the GUIClue
        if (isHand(collider))
        {
            this.collidingHand = null;
        }
    }
}
