using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class LogEntry { public string action; public string timestamp; }

public class ProgressTracker : MonoBehaviour
{
    public List<LogEntry> logs = new List<LogEntry>();

    public void Record(string action)
    {
        logs.Add(new LogEntry { action = action, timestamp = DateTime.UtcNow.ToString("o") });
        Debug.Log($"[Tracker] {action}");
    }

    public string GetJsonSummary()
    {
        return JsonUtility.ToJson(new { logs = logs.ToArray() }, true);
    }

    public void SaveJsonToFile(string filename = "session_summary.json")
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(path, GetJsonSummary());
        Debug.Log($"Saved summary to {path}");
    }
}