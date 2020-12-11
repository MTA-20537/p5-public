using UnityEngine;

public class Signifier : MonoBehaviour
{
    public bool isDiegetic = true;

    void Start()
    {
        // Start is called before the first frame update...
    }

    void Update()
    {
        // Update is called once per frame...
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
