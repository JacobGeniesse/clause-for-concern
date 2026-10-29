using UnityEngine;
using UnityEngine.InputSystem;

public class RebindTest : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction testInteract;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testInteract = inputActions["Interact"];
    }

    // Update is called once per frame
    void Update()
    {
        if (testInteract.WasPressedThisFrame())
        {
            Debug.Log("Interact");
        }
    }
}
