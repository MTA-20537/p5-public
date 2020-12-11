using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TeleportLogging : MonoBehaviour
{
    public SteamVR_Action_Boolean teleportAction;
    private LoggingManager loggingManager;
    private string objectName;
    private Vector3 lastTeleportPosition;

    void Start()
    {
        loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        objectName = gameObject.name;
        loggingManager.NewFilestamp();
        lastTeleportPosition = transform.position;
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            LogEvent("TeleportStart");
        } else if(Input.GetKeyUp(KeyCode.F))
        {
            LogEvent("TeleportEnd");
        }*/
        if (teleportAction.stateUp && loggingManager.allowLogging)
        {
            print("TELEPORT HAPPENED: " + teleportAction.updateTime);
            this.logTeleportEvent();
        }
    }

    private void logTeleportEvent()
    {
        Dictionary<string, object> otherData = new Dictionary<string, object>() {
            {"ObjectName", objectName}
            //Add things in here for extra data to be logged
        };

        otherData["endPosition"] = transform.position;
        float teleportDistance = Vector3.Distance(transform.position, lastTeleportPosition);
        otherData["teleportDistance"] = teleportDistance;
        this.lastTeleportPosition = transform.position;

        if (teleportDistance > 0) loggingManager.Log("TeleportData", otherData);
    }

    private void OnApplicationQuit()
    {

        // Tell the logging manager to save the data (to disk and SQL by default).
        loggingManager.SaveLog("TeleportData");

        // After saving the data, you can tell the logging manager to clear its logs.
        // Now its ready to save more data. Saving data will append to the existing log.
        loggingManager.ClearLog("TeleportData");
    }
}