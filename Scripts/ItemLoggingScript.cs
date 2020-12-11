using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * The class responsible for logging player item interaction events
 */
public class ItemLoggingScript : MonoBehaviour
{
    private float checkingDistance;         // the maximun distance from which a Clue can be considered noticed (this value is set in Unity, and is not defined here)
    private float FOV_MaxAngle;             // the angle resulting in the field of view from within a Clue can be noticed (this value is set in Unity, and is not defined here)

    private Transform Camera;               // the player camera
    private LoggingManager loggingManager;  // the logging manager
    private string objectName;              // the name of the associated Clue

    private float timer = 0f;               // the time that has passed since the game was started
    private float noticeTime = 0f;          // the timestamp at which the object was first noticed
    private float noticedObjectTime = 0f;   // the timestamp at which the object is considered noticable
    private float noticeTimeThreshold = 1f; // the amount of time the player must be looking at an object for it to be considered noticed
    private bool noticed = false;           // whether or not the associated Clue has been noticed
    private bool isVisible = false;         // whether or not the associated Clue is invisible

    /**
     * start is called before the first frame update
     */
    void Start()
    {
        // set up Logging Manager reference
        loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        
        // use variables declared in Logging Manager for calculations
        checkingDistance = loggingManager.checkingDistance;
        FOV_MaxAngle = loggingManager.FOV_MaxAngle;

        // retrieve player camera reference
        Camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;

        // tie name of ItemLoggingScript to name of associated Clue
        objectName = gameObject.name;

        // initialize logging
        loggingManager.NewFilestamp();
    }

    /**
     * update is called once per frame
     */
    void Update()
    {
        // find the first MeshRenderer object of the associated Clue GameObject and update the visibility of this
        CustomUtility.applyMeshRendererOperationRecursively(this.gameObject, gameObject =>
            {
                MeshRenderer objectMesh = gameObject.GetComponent<MeshRenderer>();
                this.isVisible = objectMesh.enabled;
                return; // stop at first MeshRenderer found
            }
        );

        timer += Time.deltaTime; // add the time from the previous frame onto the total time

        float FOVangle = GetFOVAngle(); // the FOV angle of the player camera (head)

        // calculate whether or not the Clue has been noticed
        if (!noticed                                                                    // the Clue must not already have been noticed
            && isVisible                                                                // the Clue must not be invisible
            && Vector3.Distance(transform.position,Camera.position) < checkingDistance  // the Clue must be within a set distance from the player
            && FOVangle < FOV_MaxAngle && loggingManager.allowLogging)                  // the Clue must be within a set angle of the center of the player's field of view
        {
            // if all above checks out, manage a timer for when the Clue has been looked at for at least one second
            if (this.noticedObjectTime == 0f) this.noticedObjectTime = timer;
            else if (timer - this.noticedObjectTime >= this.noticeTimeThreshold)
            {
                // log the "noticed" event
                Debug.Log(objectName + " noticed");
                noticed = true;
                LogEvent("Notice");
            }
        }
        else this.noticedObjectTime = 0f; // reset the timer if the Clue does no longer fulfill all the above requirements
    }

    /**
     * log the player "pickup" event
     */
    public void LogPickup()
    {
        Debug.Log(objectName + " picked up");
        LogEvent("Pickup");
    }

    /**
     * log the provided event
     */
    private void LogEvent(string eventLabel)
    {
        // define overall formatting of logs
        Dictionary<string, object> otherData = new Dictionary<string, object>() {
            {"ObjectName", objectName}
        };

        // log notice time differently depending on it's value
        if (eventLabel == "Notice")
        {
            noticeTime = timer;
            otherData["NoticeTime"] = timer;
        } else
        {
            if(noticeTime == 0)
            {
                otherData["NoticeTime"] = "NA";
            } else
            {
                otherData["NoticeTime"] = noticeTime;
            }
        }

        // log pickup events differently depending on whether the object was noticed before the pickup or not
        if (eventLabel == "Pickup" && noticeTime != 0 && noticeTime < timer)
        {
            otherData["PickupTime"] = timer;
            otherData["TimeDifference"] = timer - noticeTime;
        } else
        {
            otherData["PickupTime"] = "NA";
            otherData["TimeDifference"] = "NA";
        }

        loggingManager.Log("ItemData", otherData); // log the resulting data
    }

    /**
     * make sure to save and clean up logs when the game is quit
     */
    private void OnApplicationQuit()
    {
        // Tell the logging manager to save the data (to disk and SQL by default).
        loggingManager.SaveLog("ItemData");

        // After saving the data, you can tell the logging manager to clear its logs.
        // Now its ready to save more data. Saving data will append to the existing log.
        loggingManager.ClearLog("ItemData");
    }

    /**
     * calculate the angle difference between looking direction and direction to item
     */
    private float GetFOVAngle()
    {
        Vector3 relativeNormalizedPos = (transform.position - Camera.position).normalized;
        float dot = Vector3.Dot(relativeNormalizedPos, Camera.forward);

        //angle difference between looking direction and direction to item (radians), turned into degrees
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }
}
