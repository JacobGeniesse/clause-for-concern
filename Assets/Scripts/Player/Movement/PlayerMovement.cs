using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Input Map for Movement Controls")]
    public InputActionAsset masterList;
    private InputAction movement; //Input for movement
    private InputAction sprint; //Input for sprinting

    private Rigidbody rb; //Player's rigidbody

    [Tooltip("Rate at which the player accelerates.")]
    [SerializeField] private float accel;

    [Tooltip("Maximum speed at which the player can walk.")]
    [SerializeField] private float maxWalkSpeed;

    [Tooltip("Maximum speed at which the player can run.")]
    [SerializeField] private float maxRunSpeed;
    private bool sprinting = false; //Is the player sprinting currently?
    private Vector3 moveInputs; //Movement input
    private Vector2 currentMoveInput; // holds the current move input

    private PlayerNoiseManager playerNoiseManager;
    private bool canMove = true;

    [SerializeField, HideInInspector] private Vector3 pushForce = Vector3.zero;
    private float pushDuration;

    [Tooltip("Can the player be grabbed currently?")]
    public bool canGrab = true;
    private bool pushed = false;
    private IEnumerator pushCoroutine;

    private StunGun stunGun;

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

        try
        {
            stunGun = GetComponent<StunGun>();
        }
        catch
        {
            throw new ArgumentException("Unable to find reference to the StunGun.", nameof(PlayerMovement));
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
            velocity.y = Mathf.MoveTowards(velocity.y, velocity.y, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, moveInputs.z, maxSpeedChange);
        }

        if (pushed)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, pushForce.x, maxSpeedChange);
            velocity.y = Mathf.MoveTowards(velocity.y, velocity.y, maxSpeedChange);
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
        rb.useGravity = movability;
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
        canMove = false;
        stunGun.canShoot = false;
        pushed = true;
        yield return new WaitForSeconds(pushDuration);
        pushed = false;
        stunGun.canShoot = true;
        AlterMoveState(true);
        pushCoroutine = null;
        canGrab = true;
    }

    public void TetherObject(Transform newParent)
    {
        transform.parent = newParent;
        transform.position = newParent.position;
        rb.position = newParent.position;
    }
}
