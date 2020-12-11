using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroOutro : MonoBehaviour
{
    public Animator animator;
    public AudioManager audioManager;
    public string anim;
    private GameObject[] hands;
    public GameObject teleporter;
    public LoggingManager loggingManager;

    // Start is called before the first frame update
    void Start()
    {
        hands = GameObject.FindGameObjectsWithTag("Hand");
        playIntro();
    }

    public void playIntro()
    {
        allowPlayerTeleportation(false);
        anim = "FIn";
        audioManager.playSound("gameVO - intro");
        Invoke("playSimple", 8.00f);
    }

    public void playOutro()
    {
        //allowPlayerTeleportation(false);
        anim = "FOut";
        play(false);
        audioManager.playSound("gameVO - outro");
        Invoke("playOutro2", 22.5f);
    }

    public void playSimple()
    {
        play();
    }

    public void play(bool state = true)
    {
        animator.SetTrigger(anim);
        allowPlayerTeleportation(state);
    }
    
    public void playOutro2()
    {
        audioManager.playSound("gameVO - Resiting");
    }

    private void allowPlayerTeleportation(bool state)
    {
        print("ALLOW LOGGING? " + state);
        teleporter.SetActive(state);
        loggingManager.allowLogging = state;

        /*foreach (GameObject hand in this.hands)
        {
            hand.SetActive(state);
            CustomUtility.applyMeshRendererOperationRecursively(hand, gameObject =>
                {
                    MeshRenderer objectMesh = gameObject.GetComponent<MeshRenderer>();
                    objectMesh.enabled = state;
                }
            );
        }*/
    }
}