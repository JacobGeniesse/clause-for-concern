using UnityEngine;

public class WinConditionInteractable : MonoBehaviour, IInteractable
{
    private int taskCompletion = 0;     // Number of tasks completed 
    private int taskAmount = 4;         // Number of tasks in the level. Could probably be automated by automatically finding task objects and putting them in a list and using the list.count value

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (taskCompletion == taskAmount)
        {
            Debug.Log("A winner is you");
        }
    }

    public void IncrementTaskCompletion()
    {
        taskCompletion++;
    }

    public void IncrementTask()
    {
        // Empty function to appease the interface
    }
}
