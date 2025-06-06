using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
    string myLog = "*begin log (Press L to toggle it)";
    string filename = "";
    bool doShow = true;
    int kChars = 700;

    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }
    void Update() { if (Input.GetKeyDown(KeyCode.L)) { doShow = !doShow; } }

    public void Log(string logString, string stackTrace, LogType type)
    {
        // Prepare detailed log message
        string detailedLog = $"[{type}] {logString}";
        if (type == LogType.Error || type == LogType.Exception)
        {
            detailedLog += $"\nStack Trace:\n{stackTrace}";
        }

        // Append to on-screen log
        myLog = myLog + "\n" + detailedLog;
        if (myLog.Length > kChars)
        {
            myLog = myLog.Substring(myLog.Length - kChars);
        }

        // Append to file
        if (filename == "")
        {
            string d = System.Environment.GetFolderPath(
               System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
            System.IO.Directory.CreateDirectory(d);
            string r = Random.Range(1000, 9999).ToString();
            filename = d + "/log-" + r + ".txt";
        }
        try
        {
            System.IO.File.AppendAllText(filename, detailedLog + "\n");
        }
        catch
        {
            // Optionally handle file write errors
        }
    }

    void OnGUI()
    {
        if (!doShow) { return; }
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
           new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
        GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
    }
}
