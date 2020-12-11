using UnityEngine;

/**
 * The class responsible managing the visibility of key elements in the game according to operator input
 */
public class SignifierManager : MonoBehaviour
{
    public bool isNonDiegetic;              // whether or not this signifier is diegetic or non-diegetic
    public bool showClues;                  // whether or not clues should be rendered for the player
    public bool showCodex;                  // whether ot not the play should be shown the interactable version of the Codex or the dummy version
    public AudioManager audioManager;       // the audio manager, responsible for managing and playing audio
    private GameObject[] signifiers;        // all the Signifiers in the scene
    private Clue[] clueObjects;             // all the Clues in the scene
    private GameObject[] signObjects;       // all the signs next to the Clues in the scene
    private GameObject codexObject;         // the Codex object in the scene
    private GameObject codexDummyObject;    // the dummy version of the Codex in the scene

    /**
     * start is called before the first frame update
     */
    void Start()
    {
        // initialize objects
        signifiers = GameObject.FindGameObjectsWithTag("Signifier");
        clueObjects = (Clue[]) FindObjectsOfType(typeof(Clue));
        signObjects = GameObject.FindGameObjectsWithTag("Sign");
        codexObject = GameObject.FindGameObjectWithTag("Codex");
        codexDummyObject = GameObject.FindGameObjectWithTag("CodexDummy");

        // perform the initial update of all managed objects
        updateSignifiers();
        updateClues();
        updateCodex();
    }

    /**
     * update is called once per frame
     */
    void Update()
    {
        // switch between diegetic- and non-diegetic signifiers with "J" on the keyboard
        if (Input.GetKeyDown(KeyCode.J) && !this.isNonDiegetic)
        {
            this.isNonDiegetic = true;
            updateSignifiers();
            print("Signifiers are now NON-DIEGETIC");
        }
        else if(Input.GetKeyDown(KeyCode.J) && this.isNonDiegetic)
        {
            this.isNonDiegetic = false;
            updateSignifiers();
            print("Signifiers are now DIEGETIC");
        }

        // switch between visible or not visible Clues with "K" on the keyboard
        if (Input.GetKeyDown(KeyCode.K) && !this.showClues)
        {
            this.showClues = true;
            updateClues();
            print("Clues are now VISIBLE");

        }
        else if (Input.GetKeyDown(KeyCode.K) && this.showClues)
        {
            this.showClues = false;
            updateClues();
            print("Clues are now HIDDEN");
        }

        // switch between visible or not visible Codex with "L" on the keyboard
        if (Input.GetKeyDown(KeyCode.L) && !this.showCodex)
        {
            this.showCodex = true;
            updateCodex();
            print("Codex is now VISIBLE");

            // play go-back-to-codex VO
            audioManager.playSound("gameVO - checkcodex");
        }
        else if (Input.GetKeyDown(KeyCode.L) && this.showCodex)
        {
            this.showCodex = false;
            updateCodex();
            print("Codex is now HIDDEN");

        }
    }

    /**
     * update all individual signifiers in the scene
     */
    private void updateSignifiers()
    {
        // loop through each signifier in the scene
        foreach (GameObject sig in this.signifiers)
        {
            Signifier signifier = sig.GetComponent<Signifier>();

            // set visibility of individual signifier depending on showClues
            signifier.gameObject.SetActive(this.showClues);

            // set state of individual signifier depending on isNonDiegetic
            signifier.isDiegetic = !this.isNonDiegetic;
        }
    }

    /**
     * update all individual Clues in the scene
     */
    private void updateClues()
    {
        updateSignifiers();

        // show/hide Clues depending on showClues
        foreach (Clue clue in clueObjects)
        {
            CustomUtility.applyMeshRendererOperationRecursively(clue.gameObject, gameObject =>
                {
                    // update every MeshRenderer in every Clue object according to whether or not Clues should be displayed to the player
                    MeshRenderer objectMesh = gameObject.GetComponent<MeshRenderer>();
                    objectMesh.enabled = this.showClues;
                }
            );
        }

        // show/hide signifier signs depending on showClues
        for (int i = 0; i < this.signObjects.Length; i++)
        {
            MeshRenderer signMesh = this.signObjects[i].GetComponent<MeshRenderer>();
            signMesh.enabled = this.showClues;
        }
    }

    /**
     * switch between the two versions of the Codex according to showCodex
     */
    private void updateCodex()
    {
        // switch Codex with dummy depending on showCodex
        this.codexObject.SetActive(this.showCodex);
        this.codexDummyObject.SetActive(!this.showCodex);
    }
}
