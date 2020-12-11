using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

/**
 * The class representing a non-diegetic signifier in the game
 */
public class NDSig : MonoBehaviour
{
    public Transform player;        // the Transform object associated with the Player object
    public Transform attachedClue;  // the Clue which is being signified with this signifier
    public Vector3 offset;          // the offset of the signifier, used to allign it with the object being signified
    Interactable interactable;      // the Interactable component of the associated Clue
    public float alpha;             // the alpha value deciding to what degree the signifier should be visible by the player

    /**
     * start is called before the first frame update
     */
    void Start()
    {
        // initialize interactable
        interactable = attachedClue.gameObject.GetComponent<Interactable>();
    }

    /**
     * update is called once per frame
     */
    void Update()
    {
        // define the visibility (alpha) of the signifier based on the player's as a value between 0 and 255
        float rawDistance = Vector3.Distance(player.position, transform.position);
        float normalizedDistance= (255 / 10) * rawDistance; // 10 is arbitrary, used to regulate distance to fit the loggable notice distance
        alpha = 255 - Mathf.Clamp(normalizedDistance, 0, 255);
        
        // disable the signifier if the associated Clue has already been found
        if (attachedClue.GetComponent<Clue>().hasBeenFound)
        {
            hide();
        }
        else if (!attachedClue.GetComponent<Clue>().hasBeenFound)
        {
            show();
        }
    }

    /**
     * make the signifier invisible
     */
    public void hide()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Vector4(255, 255, 255, 0);
    }

    /**
     * make the signifier visible
     */
    public void show()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Vector4(255, 255, 255, alpha);
    }
}