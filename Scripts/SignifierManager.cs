using UnityEngine;

public class SignifierManager : MonoBehaviour
{
    public bool isNonDiegetic;          // whether or not this signifier is diegetic or non-diegetic
    public bool showClues;
    public bool showCodex;
    public AudioManager audioManager;
    private GameObject[] signifiers;
    private Clue[] clueObjects;
    private GameObject[] signObjects;   //
    private GameObject codexObject;
    private GameObject codexDummyObject;

    // Start is called before the first frame update
    void Start()
    {
        signifiers = GameObject.FindGameObjectsWithTag("Signifier");
        clueObjects = (Clue[]) FindObjectsOfType(typeof(Clue));
        signObjects = GameObject.FindGameObjectsWithTag("Sign");
        codexObject = GameObject.FindGameObjectWithTag("Codex");
        codexDummyObject = GameObject.FindGameObjectWithTag("CodexDummy");

        updateSignifiers();
        updateClues();
        updateCodex();
    }

    // Update is called once per frame
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

    private void updateSignifiers()
    {
        // update individual signifiers
        foreach (GameObject sig in this.signifiers)
        {
            Signifier signifier = sig.GetComponent<Signifier>();

            // set visibility of individual signifier depending on showClues
            signifier.gameObject.SetActive(this.showClues);

            // set state of individual signifier depending on isNonDiegetic
            signifier.isDiegetic = !this.isNonDiegetic;
        }
    }

    private void updateClues()
    {
        updateSignifiers();

        // show/hide Clues depending on showClues
        foreach (Clue clue in clueObjects)
        {
            CustomUtility.applyMeshRendererOperationRecursively(clue.gameObject, gameObject =>
                {
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

    private void updateCodex()
    {
        // switch Codex with dummy depending on showCodex
        this.codexObject.SetActive(this.showCodex);
        this.codexDummyObject.SetActive(!this.showCodex);
    }
}
