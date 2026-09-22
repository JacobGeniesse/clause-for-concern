using UnityEngine;
using UnityEngine.InputSystem;

public class TaskManager : MonoBehaviour
{
    [SerializeField] string taskName = "Unjam the Printer";
    [SerializeField] Transform location;
    [SerializeField] float timeLimit = 60f;

    public Transform Location => location;

    float timeRemaining;
    bool overdue;
    float nextStatusLog;

    void Start()
    {
        BeginTask();
    }

    void Update()
    {
        if (!overdue)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                overdue = true;
                Debug.LogWarning($"Task overdue: {taskName}");
            }
        }

        if (Time.time >= nextStatusLog)
        {
            LogStatus();
            nextStatusLog = Time.time + 10f;
        }

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            CompleteTask();
        }
    }

    void BeginTask()
    {
        timeRemaining = timeLimit;
        overdue = false;
        nextStatusLog = Time.time + 10f;
        string where = location != null ? location.name : "nowhere";
        Debug.Log($"Task started: {taskName} at {where}. {timeLimit:0}s. Press F to complete.");
    }

    void CompleteTask()
    {
        Debug.Log($"Task completed: {taskName}");
        BeginTask();
    }

    void LogStatus()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        string where = location != null ? location.name : "nowhere";
        string status = overdue ? "OVERDUE" : "Active";
        Debug.Log($"{taskName} | {where} | {seconds / 60:00}:{seconds % 60:00} | {status}");
    }

    void OnDrawGizmos()
    {
        if (location == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(location.position, 0.4f);
    }
}
