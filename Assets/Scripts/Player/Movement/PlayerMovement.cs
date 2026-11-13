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

    [Header("Movement Speed Variables")]
    [Tooltip("Rate at which the player accelerates.")]
    [SerializeField] private float accel;

    [Tooltip("Maximum speed at which the player can walk.")]
    [SerializeField] private float maxWalkSpeed;

    [Tooltip("Maximum speed at which the player can run.")]
    [SerializeField] private float maxRunSpeed;
    private bool sprinting = false; //Is the player sprinting currently?
    private Vector3 moveInputs; //Movement input
    private Vector2 currentMoveInput; // holds the current move input
    private bool canMove = true; //Can the player move?
    [SerializeField, HideInInspector] private Vector3 pushForce = Vector3.zero; //Force the player is pushed with
    
    [Tooltip("Can the player be grabbed currently? Debug variable, don't touch unless you want stuff to break")]
    public bool canGrab = true;
    private bool pushed = false; //Is the player being pushed
    private IEnumerator pushCoroutine; //Coroutine for pushing
    private StunGun stunGun; //Reference to the stun gun script
    private PlayerNoiseManager playerNoiseManager;

    private Transform originalParent;

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

        if(originalParent == null)
        {
            originalParent = transform.parent;
        }
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
        sprinting = context.performed; //Sets the bool value for sprinting
    }

    void FixedUpdate()
    {
        //Stop the player if the minimap is open
        if (Minimap.IsOpen)
        {
            Vector3 stopped = rb.linearVelocity;
            stopped.x = 0f;
            stopped.z = 0f;
            rb.linearVelocity = stopped;
            return;
        }

        if(transform.parent.transform.parent != null)
        {
            transform.position = transform.parent.transform.position;
            return;
        }


        Vector3 velocity = rb.linearVelocity; //Grab the player's current velocity
        float maxSpeedChange = accel * Time.deltaTime; //Calculate the player's acceleration

        //If the player can move and isn't being pushed
        if (canMove && !pushed)
        {
            moveInputs = Move(); //Calculate the player's movement inputs

            velocity.x = Mathf.MoveTowards(velocity.x, moveInputs.x, maxSpeedChange); //Move the current x velocity towards the desired velocity by the acceleration
            velocity.y = Mathf.MoveTowards(velocity.y, velocity.y, maxSpeedChange); //The y value shouldn't be touched to allow for gravity to function normally
            velocity.z = Mathf.MoveTowards(velocity.z, moveInputs.z, maxSpeedChange); //Move the current z velocity towards the desired velocity by the acceleration 
        }

        //If the player is being pushed
        if (pushed)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, pushForce.x, maxSpeedChange); //Move the current x velocity towards the push force's x value by the acceleration
            velocity.y = Mathf.MoveTowards(velocity.y, velocity.y, maxSpeedChange); //Don't touch the y value so gravity can function normally
            velocity.z = Mathf.MoveTowards(velocity.z, pushForce.z, maxSpeedChange); //Move the current z velocity towards the push force's z value by the acceleration
        }

        rb.linearVelocity = velocity; //Set the rigidbody's velocity equal to the calculated velocity
    }

    private void Update()
    {
        //if (Minimap.IsOpen)
        //{
        //    return;
        //}
    }

    //Function for calculating the player's movement speed
    private Vector3 Move()
    {
        Vector2 move = currentMoveInput; //Get the player's movement input

        Vector3 moveDir = transform.right * move.x + transform.forward * move.y; //Calculate the movement direction for the player taking into account the camera's point of view

        moveDir = moveDir.normalized; //Normalize the movement direction for multiplication by the movement modifier

        //If sprinting
        if(sprinting == true)
        {
            moveDir = new Vector3(moveDir.x, 0, moveDir.z) * maxRunSpeed; //Multiply the movement vector by the player's run speed
            //If the player is moving update their loud noise position
            if (move != Vector2.zero)
            {
                playerNoiseManager.UpdateLoudNoise(transform.position);
            }
        }
        else //If walking
        {
            moveDir = new Vector3(moveDir.x, 0, moveDir.z) * maxWalkSpeed; //Multiply the movement vector by the player's walk speed
            //If the player is moving update their quiet noise position
            if (move != Vector2.zero)
            {
                playerNoiseManager.UpdateQuietNoise(transform.position);
            }
        }

        return moveDir; //Return the fully calculated movement vector
    }

    //Function for altering the player's movement state
    public void AlterMoveState(bool movability)
    {
        stunGun.canShoot = movability; //Set whether the player can shoot or not
        canGrab = movability; //Set whether the player can be grabbed
        canMove = movability; //Set whether the player can move
        rb.useGravity = movability; //Set whether the player should use gravity
    }

    //Function for making the player come to a dead stop
    public void StopPlayer()
    {
        rb.linearVelocity = Vector3.zero;
    }

    //Function for pushing the player in a direction
    public void Push(Vector3 pushDir, float pushDur)
    {
        pushForce = pushDir; //Set the push force

        //Start the pushing coroutine
        if (pushCoroutine == null)
        {
            pushCoroutine = ExecutePush(pushDur);
            StartCoroutine(pushCoroutine);
        }
    }

    //Coroutine for executing a push
    private IEnumerator ExecutePush(float pushDuration)
    {
        canGrab = false; //Make the player un-grabbable
        canMove = false; //Make it so the player cannot move
        pushed = true; //Mark the player as being pushed
        yield return new WaitForSeconds(pushDuration); //Wait for the duration of the push
        pushed = false; //Mark the player as no longer being pushed
        AlterMoveState(true); //Free the player's movement and shooting capability
        pushCoroutine = null; //Set the push coroutine equal to null for future use
    }

    //Function for tethering the player to another object (Used in grab attacks)
    public void TetherObject(Transform newParent)
    {
        transform.parent = newParent; //Set the transform for the parent
        transform.position = newParent.position; //Set the player's position
        rb.position = newParent.position; //Set the rigidbody as a failsafe since this stuff is touchy for some reason.
    }

    public void ReturnToOriginalParent()
    {
        transform.parent = originalParent;
    }
}
