using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class NDSig : MonoBehaviour
{
    public Transform player;
    public Transform attachedClue;
    public Vector3 offset;
    Interactable interactable;
    Color invis = new Vector4(255, 255, 255, 0); // will be overwritten on update
    Color vis = new Vector4(255, 255, 255, 255); // will be overwritten on update
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        interactable = attachedClue.gameObject.GetComponent<Interactable>();
    }
    
    // Update is called once per frame
    void Update()
    {
        float rawDistance = Vector3.Distance(player.position, transform.position);
        float normalizedDistance= (255 / 10) * rawDistance; // 2 is arbitrary
        distance = 255 - Mathf.Clamp(normalizedDistance, 0, 255);
        
        if (attachedClue.GetComponent<Clue>().hasBeenFound)
        {
            gameObject.GetComponent<SpriteRenderer>().color = invis;
        }
        else if (!attachedClue.GetComponent<Clue>().hasBeenFound)
        {
            vis = new Vector4(255, 255, 255, distance);
            gameObject.GetComponent<SpriteRenderer>().color = vis;
        }
    }
    public void hide()
    {
        gameObject.GetComponent<SpriteRenderer>().color = invis;
    }
    public void show()
    {
        gameObject.GetComponent<SpriteRenderer>().color = vis;
    }
}