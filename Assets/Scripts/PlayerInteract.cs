using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerInteract : MonoBehaviour
{
    public InputActionAsset MasterList;

    private InputAction interact;

    [SerializeField]
    private float interactRange = 3f;

    private Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interact = MasterList["Interact"];

        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Interaction();
    }

    void Interaction()
    {
        // Make it so it only detects the button press on the frame the button was pressed
        if (interact.WasPressedThisFrame())
        {
            if (camera != null)
            {
                Ray ray = new Ray(camera.transform.position, camera.transform.forward);

                if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
                {
                    if (hit.collider.TryGetComponent(out IInteractable interactable))   // Check if the the collider object has a script that inherets from the IInteractable interface
                    {
                        interactable.Interact();
                    }
                }
            }
        }
    }

    
}
