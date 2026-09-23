using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private string[] taskNames;
    [SerializeField] private string[] taskLocation;
    [SerializeField] private TaskInteractable[] taskObjects;
    private int currentTask;


    [SerializeField] string taskName = "Unjam the Printer";
    private Transform location;

    private float timerPenalty = 0;
    [SerializeField] float addedPenalty = 5f;

    [SerializeField] float timeLimit = 60f;

    public Transform Location => location;

    private int lastNum;

    float timeRemaining;
    bool overdue;
    float nextStatusLog;

    private EnemyBehavior enemyBehavior;

    void Start()
    {
        try
        {
            enemyBehavior = GameObject.Find("Enemy").GetComponent<EnemyBehavior>();
        }
        catch
        {
            throw new ArgumentException("Unable to find EnemyBehavior.", nameof(TaskManager));
        }

        if (!HasTasks())
        {
            Debug.LogError("TaskManager has empty or mismatched task lists.");
            enabled = false;
            return;
        }

        lastNum = taskNames.Length + 1;

        BeginTask();
    }

    void Update()
    {
        if (!overdue)
        {
            enemyBehavior.aggressive = false;
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                overdue = true;
                Debug.LogWarning($"Task overdue: {taskName}");
            }
        }
        else
        {
            enemyBehavior.aggressive = true;
            timerPenalty = 0;
        }

        if (Time.time >= nextStatusLog)
        {
            LogStatus();
            nextStatusLog = Time.time + 10f;
        }

        //if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        //{
        //    CompleteTask();
        //}
    }

    bool HasTasks()
    {
        int count = taskNames != null ? taskNames.Length : 0;
        return count > 0
            && taskLocation != null && taskLocation.Length == count
            && taskObjects != null && taskObjects.Length == count;
    }

    void BeginTask()
    {
        if (!HasTasks())
        {
            return;
        }
        RandomizeTasks();
        taskObjects[currentTask].currentTask = true;
        timeRemaining = timeLimit - timerPenalty;
        overdue = false;
        nextStatusLog = Time.time + 10f;
        location = taskObjects[currentTask].transform;
        string where = location != null ? location.name : "nowhere";
        Debug.Log($"Task started: {taskNames[currentTask]} at {taskLocation[currentTask]}. {timeLimit:0}s.");
    }

    public void CompleteTask()
    {
        taskObjects[currentTask].currentTask = false;
        Debug.Log($"Task completed: {taskNames[currentTask]}");
        timerPenalty += addedPenalty;
        BeginTask();
    }

    void LogStatus()
    {
        if (!HasTasks())
        {
            return;
        }

        int seconds = Mathf.CeilToInt(timeRemaining);
        string where = location != null ? location.name : "nowhere";
        string status = overdue ? "OVERDUE" : "Active";
        Debug.Log($"{taskNames[currentTask]} | {taskLocation[currentTask]} | {seconds / 60:00}:{seconds % 60:00} | {status}");
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

    void RandomizeTasks()
    {
        currentTask = UnityEngine.Random.Range(0, taskNames.Length);
        if (currentTask == lastNum)
        {
            RandomizeTasks();
        }
        lastNum = currentTask;
    }
}
