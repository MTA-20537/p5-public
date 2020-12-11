using System;
using System.Linq;
using UnityEngine;

/**
 * The class responsible for managing audio playback in the game
 */
public class AudioManager : MonoBehaviour
{
    public AudioClip[] voiceLines;  // all the voice lines of the game (this is declared in the Unity editor and not in the code)
    private string[] names;         // the names of all the different audio clips
    private string lastPlayed;      // the name of the audio clip that was most recently played

    /**
     * start is called before the first frame update
     */
    void Start()
    {
        // declare list of audio clip names
        this.names = new string[voiceLines.Length];
        this.names = this.voiceLines.Select(voiceLines => voiceLines.name).ToArray();

        this.lastPlayed = "";
    }

    /**
     * play an audio clip with the given name
     */
    public void playSound(string name)
    {
        try
        {
            int targetIndex = -1;
            for (int i = 0; i < this.names.Length; i++) if (this.names[i] == name) targetIndex = i;
            this.playSound(targetIndex);
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.LogError("Could not parse and play sound file \"" + name + "\": " + e);
        }
    }

    /**
     * play a random "correct" audio clip
     */
    public void playRandomCorrectSound()
    {
        string[] corrects = { "gameVO - makessense", "gameVO - seemsright", "gameVO - yes" };
        this.playRandomSound(corrects);
    }

    /**
     * play a random "wrong" audio clip
     */
    public void playRandomWrongSound()
    {
        string[] wrongs = { "gameVO - no1", "gameVO - no2", "gameVO - noway", "gameVO - cmonthink", "gameVO - doesntseemright", "gameVO - itcouldmakesense" };
        this.playRandomSound(wrongs);
    }

    /**
     * play an audio clip with the given index
     */
    private void playSound(int index)
    {
        this.gameObject.GetComponent<AudioSource>().Stop(); // make sure to halt any audio clips currently playing
        this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.voiceLines[index], 0.4f);
    }

    /**
     * play a random audio clip, and ensure that the same clip is not played twice in a row
     */
    private void playRandomSound(string[] choices)
    {
        // select a random sound from the given choices, and make sure the same sound is not played twice in a row
        string chosenSound = choices[UnityEngine.Random.Range(0, choices.Length)];
        while (choices.Length > 1 && chosenSound == this.lastPlayed) chosenSound = choices[UnityEngine.Random.Range(0, choices.Length)];
        this.lastPlayed = chosenSound;

        // play randomly selected sound
        this.playSound(chosenSound);
    }
}
