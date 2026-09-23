using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    private bool _isInteractable = true;

    private WinConditionInteractable win;

    private void Start()
    {
        win = GameObject.Find("EscapeElevatorDoors").GetComponent<WinConditionInteractable>();
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
