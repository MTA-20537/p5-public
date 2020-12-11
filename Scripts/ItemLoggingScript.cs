using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemLoggingScript : MonoBehaviour
{
    private float checkingDistance;
    private float FOV_MaxAngle;

    private Transform Camera;
    private LoggingManager loggingManager;
    private string objectName;

    private float timer = 0f;
    private float noticeTime = 0f;
    private float noticedObjectTime = 0f;
    private float noticeTimeThreshold = 1f;
    private bool noticed = false;
    private bool isVisible = false;

    void Start()
    {
        loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        
        checkingDistance = loggingManager.checkingDistance;
        FOV_MaxAngle = loggingManager.FOV_MaxAngle;

        Camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;

        objectName = gameObject.name;

        loggingManager.NewFilestamp();
    }

    void Update()
    {
        CustomUtility.applyMeshRendererOperationRecursively(this.gameObject, gameObject =>
            {
                MeshRenderer objectMesh = gameObject.GetComponent<MeshRenderer>();
                this.isVisible = objectMesh.enabled;
                return; // stop at first MeshRenderer found
            }
        );

        timer += Time.deltaTime;

        float FOVangle = GetFOVAngle();

        if (!noticed 
            && isVisible 
            && Vector3.Distance(transform.position,Camera.position)<checkingDistance 
            && FOVangle<FOV_MaxAngle && loggingManager.allowLogging)
        {
            if (this.noticedObjectTime == 0f) this.noticedObjectTime = timer;
            else if (timer - this.noticedObjectTime >= this.noticeTimeThreshold)
            {
                Debug.Log(objectName + " noticed");
                noticed = true;
                LogEvent("Notice");
            }
        }
        else this.noticedObjectTime = 0f;

        /* For testing LogPickup
        if(Input.GetKeyDown(KeyCode.Q))
        {
            LogPickup();
        }*/
    }

    //Add this function to be called by the throwable whenever picked up
    public void LogPickup()
    {
        Debug.Log(objectName + " picked up");
        LogEvent("Pickup");
    }

    private void LogEvent(string eventLabel)
    {
        Dictionary<string, object> otherData = new Dictionary<string, object>() {
            {"ObjectName", objectName}
            //Add things in here for extra data to be logged
        };

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

        if (eventLabel == "Pickup" && noticeTime != 0 && noticeTime < timer)
        {
            otherData["PickupTime"] = timer;
            otherData["TimeDifference"] = timer - noticeTime;
        } else
        {
            otherData["PickupTime"] = "NA";
            otherData["TimeDifference"] = "NA";
        }

        loggingManager.Log("ItemData", otherData);
    }

    private void OnApplicationQuit()
    {

        // Tell the logging manager to save the data (to disk and SQL by default).
        loggingManager.SaveLog("ItemData");

        // After saving the data, you can tell the logging manager to clear its logs.
        // Now its ready to save more data. Saving data will append to the existing log.
        loggingManager.ClearLog("ItemData");
    }

    private float GetFOVAngle()
    {
        Vector3 relativeNormalizedPos = (transform.position - Camera.position).normalized;
        float dot = Vector3.Dot(relativeNormalizedPos, Camera.forward);

        //angle difference between looking direction and direction to item (radians), turned into degrees
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }


    /* LEGACY CODE
    private void LogObjectNotice()
    {
        if(!noticed)
        {
            Debug.Log("Object Noticed!");
            noticed = true;
            noticeTime = timer;
        }
    }

    private void LogPickup()
    {
        pickupTime = timer;
        Log(noticeTime, pickupTime);
    }

    private void Log(float NoticeTime, float PickupTime)
    {
        string filePath = getPath();
        float timeDifference = PickupTime - NoticeTime;
        //This is the writer, it writes to the filepath
        StreamWriter writer = new StreamWriter(filePath);

        writer.WriteLine(gameObject.name + "," + NoticeTime + "," + PickupTime + "," + timeDifference);
        writer.Flush();
        writer.Close();
    }

    private string getPath()
    {
        #if UNITY_EDITOR
            return Application.dataPath + "/Logging/" + "Object_Times.csv";
        #elif UNITY_ANDROID
            return Application.persistentDataPath+"Object_Times.csv";
        #elif UNITY_IPHONE
            return Application.persistentDataPath+"/"+"Object_Times.csv";
        #else
            return Application.dataPath +"/"+"Object_Times.csv";
        #endif
    }*/
}
