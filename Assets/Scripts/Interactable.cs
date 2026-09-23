using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    private bool _isInteractable = true;

    [SerializeField] private GameObject inputPopup;
    private Transform playerTrans;

    [SerializeField] private float minDistance = 10;


    private WinConditionInteractable win;

    private void Start()
    {
        win = GameObject.Find("EscapeElevatorDoors").GetComponent<WinConditionInteractable>();

        if (playerTrans == null)
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
    }

    void Update()
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);

        if (distanceToPlayer < minDistance)
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
        Debug.Log("Interaction 1");
        if (_isInteractable)
        {
            IncrementTask();
        }
        _isInteractable = false;
    }

    public void IncrementTask()
    {
        if (win != null)
        {
            win.IncrementTaskCompletion();
            this.gameObject.SetActive(false);
        }
    }
}
