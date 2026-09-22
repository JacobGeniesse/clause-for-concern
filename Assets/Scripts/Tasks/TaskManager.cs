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

    float timeRemaining;
    bool overdue;
    float nextStatusLog;

    [SerializeField] private EnemyBehavior enemyBehavior;

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

    void BeginTask()
    {
        currentTask = UnityEngine.Random.Range(0, taskNames.Length);
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
}
