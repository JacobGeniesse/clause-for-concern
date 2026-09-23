using UnityEngine;

public class WinConditionInteractable : MonoBehaviour, IInteractable
{
    private int taskCompletion = 0;     // Number of tasks completed 
    [SerializeField] private int taskAmount = 4;         // Number of tasks in the level. Could probably be automated by automatically finding task objects and putting them in a list and using the list.count value

    private GameManager gameManager;
    [SerializeField] private GameObject winScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Interact()
    {
        if (taskCompletion == taskAmount)
        {
            gameManager.ModTime(0);
            Cursor.lockState = CursorLockMode.None;
            winScreen.SetActive(true);
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
