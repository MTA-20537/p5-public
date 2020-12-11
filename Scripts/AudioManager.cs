using System;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] voiceLines;
    private string[] names;
    private string lastPlayed;

    // Start is called before the first frame update
    void Start()
    {
        this.names = new string[voiceLines.Length];
        this.names = this.voiceLines.Select(voiceLines => voiceLines.name).ToArray();
        this.lastPlayed = "";
    }

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

    public void playRandomCorrectSound()
    {
        string[] corrects = { "gameVO - makessense", "gameVO - seemsright", "gameVO - yes" };
        this.playRandomSound(corrects);
    }

    public void playRandomWrongSound()
    {
        string[] wrongs = { "gameVO - no1", "gameVO - no2", "gameVO - noway", "gameVO - cmonthink", "gameVO - doesntseemright", "gameVO - itcouldmakesense" };
        this.playRandomSound(wrongs);
    }

    private void playSound(int index)
    {
        this.gameObject.GetComponent<AudioSource>().Stop();
        this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.voiceLines[index], 0.4f);
    }

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
