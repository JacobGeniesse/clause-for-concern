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
    private Vector3 moveDirection;

    private PlayerNoiseManager playerNoiseManager;

    void Start()
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

    void FixedUpdate()
    {
        moveDirection = CalcDirection();

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
    }

    private Vector3 Move()
    {
        Vector2 move = movement.ReadValue<Vector2>();

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

    private Vector3 CalcDirection()
    {
        Vector2 move = movement.ReadValue<Vector2>();

        return transform.right * move.x + transform.forward * move.y;
    }
}
