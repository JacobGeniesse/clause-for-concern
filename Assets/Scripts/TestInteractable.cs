using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    private bool _isInteractable = true;
    public void Interact()
    {
        Debug.Log("Interaction 2");
        if (_isInteractable )
        {
            IncrementTask();
        }
        _isInteractable=false;
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
