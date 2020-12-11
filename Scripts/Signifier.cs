using UnityEngine;

/**
 * The class representing a diegetic signifier in the game
 */
public class Signifier : MonoBehaviour
{
    public bool isDiegetic = true;  // whether the desired signifers should be diegetic or non-diegetic

    /**
     * update is called once per frame
     */
    void Update()
    {
        // show or hide diegetic signifiers depending on isDiegetic
        foreach(Transform child in transform)
        {
            if(child.gameObject.tag == "Diegetic")
            {
                child.gameObject.SetActive(this.isDiegetic);
            }
            else
            {
                child.gameObject.SetActive(!this.isDiegetic);
            }
        }
    }
}
