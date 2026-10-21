using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public InputActionAsset MasterList;

    private InputAction interact;

    [SerializeField]
    private float interactRange = 4f;

    private Camera playerCamera;

    void Awake()
    {
        interact = MasterList["Interact"];
        playerCamera = Camera.main;
    }

    void OnEnable(){
        interact.Enable(); // turns on the interact action so it can send inputs
        interact.performed += OnInteract; // calls OnInteract when the interact input is performed
    }

    void OnDisable(){
        interact.performed -= OnInteract; // stops OnInteract from being called on interact input performed
        interact.Disable(); // turns off the interact action so it can't send inputs
    }

    private void OnInteract(InputAction.CallbackContext context){ // called when the interact input is performed
        Interaction(); // calls the Interaction method
    }


    void Interaction()
    {

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position + playerCamera.transform.forward * 0.6f,
            playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactRange);
        if (hits.Length == 0)
        {
            return;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
                return;
            }
        }
    }
}
