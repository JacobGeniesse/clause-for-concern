using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interaction 1");
    }
}
