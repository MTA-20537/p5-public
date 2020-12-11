using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/**
 * The class responsible for logging player teleportation events
 */
public class TeleportLogging : MonoBehaviour
{
    public SteamVR_Action_Boolean teleportAction;   // a listener for the state of the button on the controller associated with teleportation
    private LoggingManager loggingManager;          // the logging manager object
    private string objectName;                      // the name of the associated GameObject
    private Vector3 lastTeleportPosition;           // the coordinates of the last teleportation target

    /**
     * start is called before the first frame update
     */
    void Start()
    {
        loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();  // the logging manager object reference
        objectName = gameObject.name;                                                       // the object name reference
        loggingManager.NewFilestamp();                                                      // initialize logging
        lastTeleportPosition = transform.position;                                          // the last teleportation is initialized as the player's initial position when starting the game
    }


    /** 
     * update is called once per frame
     */
    void Update()
    {
        // only log teleportation events if it just happended and logging is enabled
        if (teleportAction.stateUp && loggingManager.allowLogging)
        {
            print("TELEPORT HAPPENED: " + teleportAction.updateTime);
            this.logTeleportEvent();
        }
    }

    /**
     * logs a teleport event
     */
    private void logTeleportEvent()
    {
        // define the formatting of the logs
        Dictionary<string, object> otherData = new Dictionary<string, object>() {
            {"ObjectName", objectName}
        };

        // use the player's position after a successful teleport as the target position
        otherData["endPosition"] = transform.position;
        // calculate the distance teleported
        float teleportDistance = Vector3.Distance(transform.position, lastTeleportPosition);
        otherData["teleportDistance"] = teleportDistance;                 
        // remember the current position for next teleportation event
        this.lastTeleportPosition = transform.position;

        // do not log failed teleportation attempts
        if (teleportDistance > 0) loggingManager.Log("TeleportData", otherData);
    }

    /**
     * make sure to save and clean up logs when the game is quit
     */
    private void OnApplicationQuit()
    {
        // tell the logging manager to save the data (to disk and SQL by default).
        loggingManager.SaveLog("TeleportData");

        // after saving the data, you can tell the logging manager to clear its logs.
        // now its ready to save more data. Saving data will append to the existing log.
        loggingManager.ClearLog("TeleportData");
    }
}