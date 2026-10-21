using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    public InputActionAsset masterList;

    private InputAction movement;
    private InputAction sprint;

    private Rigidbody rb;

    [SerializeField]
    private float accel;

    [SerializeField]
    private float maxWalkSpeed;

    [SerializeField]
    private float maxRunSpeed;
    private bool sprinting = false;

    private Vector3 moveInputs;

    private Vector2 currentMoveInput; // holds the current move input

    private PlayerNoiseManager playerNoiseManager;

    void Awake()// called when the script instance is being loaded, lookups happen before onEnable runs
    {
        try
        {
            movement = masterList["Move"];
            sprint = masterList["Sprint"];
        }
        catch
        {
            throw new ArgumentException("Unable to bind movement keys! Check if masterList var is assigned properly!", nameof(PlayerMovement));
        }

        try
        {
            rb = GetComponent<Rigidbody>();
        }
        catch
        {
            throw new ArgumentException("Unable to find rigidbody!", nameof(PlayerMovement));
        }

        try
        {
            playerNoiseManager = GetComponent<PlayerNoiseManager>();
        }
        catch
        {
            throw new ArgumentException("Unable to find player noise manager!", nameof(PlayerMovement));
        }
    }

    void OnEnable(){
        movement.Enable(); // turns on the move action so it can send inputs
        sprint.Enable(); // turns on the sprint action so it can send inputs

        movement.performed += OnMove; // calls OnMove when the move input changes
        movement.canceled += OnMove; // calls OnMove when the move input is released
        sprint.performed += OnSprint; // calls OnSprint when sprint key is pressed
        sprint.canceled += OnSprint; // calls OnSprint when sprint key is released
    }

    void OnDisable(){
        movement.performed -= OnMove; //Stops OnMove from being called on move input change
        movement.canceled -= OnMove; //Stops OnMove from being called on move input release
        sprint.performed -= OnSprint; //Stops OnSprint from being called on sprint press
        sprint.canceled -= OnSprint; //Stops OnSprint from being called on sprint release

        movement.Disable(); //Turns off the move action so it can't send inputs
        sprint.Disable(); //Turns off the sprint action so it can't send inputs
    }

    private void OnMove(InputAction.CallbackContext context){
        currentMoveInput = context.ReadValue<Vector2>(); // gets the current move input (0,0) when released
    }

    private void OnSprint(InputAction.CallbackContext context){
        sprinting = context.performed;
    }

    void FixedUpdate()
    {
        if (Minimap.IsOpen)
        {
            Vector3 stopped = rb.linearVelocity;
            stopped.x = 0f;
            stopped.z = 0f;
            rb.linearVelocity = stopped;
            return;
        }

        //moveDirection = CalcDirection();

        moveInputs = Move();

        Vector3 velocity = rb.linearVelocity;

        float maxSpeedChange = accel * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, moveInputs.x, maxSpeedChange);
        velocity.y = Mathf.MoveTowards(velocity.y, moveInputs.y, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, moveInputs.z, maxSpeedChange);

        rb.linearVelocity = velocity;
    }

    private void Update()
    {
        if (Minimap.IsOpen)
        {
            return;
        }

        /*
        if (sprint.WasPressedThisFrame() == true)
        {
            if (sprinting == false)
            {
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }
        }
        */
    }

    private Vector3 Move()
    {
        Vector2 move = currentMoveInput;

        Vector3 moveDir = transform.right * move.x + transform.forward * move.y;

        moveDir = moveDir.normalized;

        if(sprinting == true)
        {
            moveDir = new Vector3(moveDir.x, 0, moveDir.z) * maxRunSpeed;
            if (move != Vector2.zero)
            {
                playerNoiseManager.UpdateLoudNoise(transform.position);
            }
        }
        else
        {
            moveDir = new Vector3(moveDir.x, 0, moveDir.z) * maxWalkSpeed;
            if (move != Vector2.zero)
            {
                playerNoiseManager.UpdateQuietNoise(transform.position);
            }
        }

        return moveDir;
    }

}
