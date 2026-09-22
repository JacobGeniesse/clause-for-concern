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

    private float xRot = 0f;

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
    void LateUpdate()
    {
        float mouseX = mouseInputX.ReadValue<float>() * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInputY.ReadValue<float>() * mouseSensitivity * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90, 90);

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
