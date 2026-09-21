using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    private bool _isInteractable = true;
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
        WinConditionInteractable win = GameObject.Find("EscapeElevatorDoors").GetComponent<WinConditionInteractable>();
        if (win != null)
        {
            win.IncrementTaskCompletion();
        }
    }
}
