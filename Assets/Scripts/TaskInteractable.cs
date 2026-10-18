using UnityEngine;

public class TaskInteractable : MonoBehaviour, IInteractable
{
    public bool currentTask = false;

    [SerializeField] private GameObject inputPopup;
    private Transform playerTrans;
    [SerializeField] private float minDistance = 10;

    private TaskManager taskManager;

    void Start()
    {
        if (taskManager == null)
        {
            taskManager = GameObject.Find("GameManager").GetComponent<TaskManager>();
        }

        if(playerTrans == null)
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
    }


    void Update()
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);

        if(distanceToPlayer < minDistance && currentTask == true)
        {
            inputPopup.SetActive(true);
        }
        else
        {
            inputPopup.SetActive(false);
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
