using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity;

    [SerializeField]
    private float controllerSensitivity;

    [SerializeField]
    private Transform playerTrans;

    [SerializeField]
    private PlayerInput playerInput;

    private float xRot;

    public InputActionAsset MasterList;

    private InputAction mouseInputX;
    private InputAction mouseInputY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mouseInputX = MasterList["MouseX"];
        mouseInputY = MasterList["MouseY"];

        LockCursor();


    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = mouseInputX.ReadValue<float>() * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInputY.ReadValue<float>() * mouseSensitivity * Time.deltaTime;

        float desiredX = playerTrans.localRotation.eulerAngles.y + mouseX;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90, 90);

        playerTrans.localRotation = Quaternion.Euler(0, desiredX, 0);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        playerTrans.Rotate(Vector3.up * mouseX);
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
