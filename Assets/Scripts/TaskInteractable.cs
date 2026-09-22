using UnityEngine;

public class TaskInteractable : MonoBehaviour, IInteractable
{
    public bool currentTask = false;

    private TaskManager taskManager;

    void Start()
    {
        if (taskManager == null)
        {
            taskManager = GameObject.Find("GameManager").GetComponent<TaskManager>();
        }
    }


    public void Interact()
    {
        if (currentTask == true)
        {
            IncrementTask();
        }
    }

    public void IncrementTask()
    {
        taskManager.CompleteTask();
    }
}
