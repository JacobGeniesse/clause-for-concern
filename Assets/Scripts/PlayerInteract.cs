using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public InputActionAsset MasterList;

    private InputAction interact;

    [SerializeField]
    private float interactRange = 4f;

    private Camera playerCamera;

    void Start()
    {
        interact = MasterList["Interact"];
        interact.Enable();
        playerCamera = Camera.main;
    }

    void Update()
    {
        Interaction();
    }

    void Interaction()
    {
        if (interact == null || !interact.WasPressedThisFrame())
        {
            return;
        }

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
