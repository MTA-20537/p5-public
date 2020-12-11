using UnityEngine;
using UnityEngine.UI;

public class Codex : MonoBehaviour
{
    public GUIClue[] clues;                     // the amount of clues in the scene
    public int cluesFound;                      // the total amount of clues found so far
    public bool gameHasBeenWon;                 // whether or not the game has been won
    public AudioClip newClueUnlockedSound;      // the sound effect that plays when the player unlocks a new clue
    public AudioClip gameWonSound;              // the sound effect that plays when the player wins the game
    public IntroOutro introOutro;

    // Start is called before the first frame update
    void Start()
    {
        this.cluesFound = 0;            // start the game with zero clues found
        this.gameHasBeenWon = false;    // start the game in the "not-won"-state
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1f, 0.6f, 0));
    }

    // Update is called once per frame
    void Update()
    {
        // update CluesFoundText
        this.gameObject.transform.GetChild(8).gameObject.GetComponent<Text>().text = this.cluesFound + "/" + this.clues.Length + " CLUES FOUND";

        // update the rest of the dynamic text
        if (this.gameHasBeenWon)
        {
            // show the "you won the game"-screen when the game has been won
            foreach (GUIClue clue in clues)
            {
                clue.isInteractable = false; // this probably doesn't matter, since the GUIClue will also be moved off screen...
                clue.gameObject.transform.position = new Vector3(transform.position.x, -100, transform.position.z);                                 // move GUIClue out of player view
            }
            this.gameObject.transform.GetChild(0).gameObject.transform.position = new Vector3(transform.position.x, 1.6f, transform.position.z);    // show victory text

            this.gameObject.transform.GetChild(1).gameObject.transform.position = new Vector3(0.0f, -100, transform.position.z);                    // show green background panel
            this.gameObject.transform.GetChild(2).gameObject.transform.position = new Vector3(-0.35f, -100, transform.position.z);                  // show red background panel

            this.gameObject.transform.GetChild(5).gameObject.transform.position = new Vector3(0.35f, -100, transform.position.z);                   // hide unassigned text
            this.gameObject.transform.GetChild(6).gameObject.transform.position = new Vector3(0.0f, -100, transform.position.z);                    // hide useful text
            this.gameObject.transform.GetChild(7).gameObject.transform.position = new Vector3(-0.35f, -100, transform.position.z);                  // hide useless text
            return;
        }
        else
        {
            // show all the clues discovered so far and the categories if the game is not yet won
            this.gameObject.transform.GetChild(0).gameObject.transform.position = new Vector3(transform.position.x, -100, transform.position.z);    // show victory text

            this.gameObject.transform.GetChild(1).gameObject.transform.position = new Vector3(0.0f, 1.54f, transform.position.z);                   // hide green background panel
            this.gameObject.transform.GetChild(2).gameObject.transform.position = new Vector3(-0.35f, 1.54f, transform.position.z);                 // hide red background panel

            this.gameObject.transform.GetChild(5).gameObject.transform.position = new Vector3(0.35f, 1.82f, transform.position.z);                  // hide useful text
            this.gameObject.transform.GetChild(6).gameObject.transform.position = new Vector3(0.0f, 1.82f, transform.position.z);                   // show useful text
            this.gameObject.transform.GetChild(7).gameObject.transform.position = new Vector3(-0.35f, 1.82f, transform.position.z);                 // show useless text
        }

        // show the "clues will appear here" helping text in the UNASSIGNED column if no clues are yet to be found, otherwise move it our of view
        if (this.cluesFound == 0)
        {
            this.gameObject.transform.GetChild(4).gameObject.transform.position = new Vector3(0.37f, 1.55f, transform.position.z);                  // show friendly helper text
        }
        else
        {
            this.gameObject.transform.GetChild(4).gameObject.transform.position = new Vector3(0.37f, -100, transform.position.z);                    // hide friendly helper text
        }

        // figure out what clues are in what category and display them
        int updatedCluesFound = 0;
        GUIClue clueBeingHeld = null;
        (int unassigned, int useful, int useless) cluesInCategories = (0, 0, 0);
        bool allCluesInCorrectCategory = true;

        foreach (GUIClue clue in clues)
        {
            if (allCluesInCorrectCategory) allCluesInCorrectCategory = clue.clue.isUseful ? clue.category == 1 : clue.category == 2;    // check if GUIClue is placed in the correct category (must be placed in either USEFUL OR USELESS)
            if (clue.clue.hasBeenFound) updatedCluesFound++;                                                                            // if a Clue has been found, add it to the count of Clues found
            else continue;                                                                                                              // avoid updating GUIClue position if it has not been found to keep it out of view of the player
            // the the currect GUIClue is not being held, place it into its specified category
            if (!clue.isHolding)
            {
                switch (clue.category)
                {
                    case 1:     // USEFUL
                        cluesInCategories.useful++;
                        clue.gameObject.transform.position = new Vector3(0.0f, 1.8f - (0.08f * cluesInCategories.useful), transform.position.z);        // TODO: fine tune offset
                        break;
                    case 2:     // USELESS
                        cluesInCategories.useless++;
                        clue.gameObject.transform.position = new Vector3(-0.35f, 1.8f - (0.08f * cluesInCategories.useless), transform.position.z);     // TODO: fine tune offset
                        break;
                    default:    // UNASSIGNED
                        cluesInCategories.unassigned++;
                        clue.gameObject.transform.position = new Vector3(0.35f, 1.8f - (0.08f * cluesInCategories.unassigned), transform.position.z);   // TODO: fine tune offset
                        break;
                }
            }
            else
            {
                clueBeingHeld = clue;
            }
        }

        // go into win state if all the clues are in the correct category
        if (allCluesInCorrectCategory && !clueBeingHeld)
        {
            this.gameHasBeenWon = true;                                                 // if all the GUIClues were placed in the correct categories, the game has been won
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.gameWonSound); // play victory sound for extra celebratory effect
            introOutro.playOutro();
        }

        // if the player is already holding a GUIClue, they may not be allowed to pick up another one
        foreach (GUIClue clue in clues)
        {
            if (clueBeingHeld)
            {
                if (clue.clue.identifier != clueBeingHeld.clue.identifier)
                {
                    clue.isInteractable = false;
                }
            }
            else
            {
                clue.isInteractable = true;
            }
        }

        // update the amount of clues found and play sound effect if a new clue has been discovered since last update
        //if (updatedCluesFound > this.cluesFound) this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.newClueUnlockedSound);
        this.cluesFound = updatedCluesFound;
    }
}
