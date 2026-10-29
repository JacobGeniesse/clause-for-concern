using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private bool canMove = true;

    [SerializeField] private Vector3 pushForce = Vector3.zero;
    private float pushDuration;

    public bool canGrab = true;
    private bool pushed = false;
    private IEnumerator pushCoroutine;

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
            canMove = false;
            Vector3 stopped = rb.linearVelocity;
            stopped.x = 0f;
            stopped.z = 0f;
            rb.linearVelocity = stopped;
            return;
        }
        else
        {
            canMove = true;
        }


        Vector3 velocity = rb.linearVelocity;
        float maxSpeedChange = accel * Time.deltaTime;

        if (canMove && !pushed)
        {
            moveInputs = Move();

            velocity.x = Mathf.MoveTowards(velocity.x, moveInputs.x, maxSpeedChange);
            velocity.y = Mathf.MoveTowards(velocity.y, moveInputs.y, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, moveInputs.z, maxSpeedChange);
        }

        if (pushed)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, pushForce.x, maxSpeedChange);
            velocity.y = Mathf.MoveTowards(velocity.y, pushForce.y, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, pushForce.z, maxSpeedChange);
        }

        rb.linearVelocity = velocity;
    }

    private void Update()
    {
        if (Minimap.IsOpen)
        {
            return;
        }
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
    public void AlterMoveState(bool movability)
    {
        canGrab = movability;
        canMove = movability;
    }

    public void StopPlayer()
    {
        rb.linearVelocity = Vector3.zero;
    }

    public void Push(Vector3 pushDir, float pushDur)
    {
        pushForce = pushDir;
        if (pushCoroutine == null)
        {
            pushCoroutine = ExecutePush(pushDur);
            StartCoroutine(pushCoroutine);
        }
    }

    private IEnumerator ExecutePush(float pushDuration)
    {
        canGrab = false;
        pushed = true;
        yield return new WaitForSeconds(pushDuration);
        pushed = false;
        AlterMoveState(true);
        pushCoroutine = null;
        canGrab = true;
    }

    public void TetherObject(Transform newParent)
    {
        transform.parent = newParent;
        rb.position = newParent.position;
    }
}
