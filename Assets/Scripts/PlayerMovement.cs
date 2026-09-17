using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    public InputActionAsset MasterList;

    private InputAction movement;

    private Rigidbody rb;

    [SerializeField]
    private float accel;

    [SerializeField]
    private float maxSpeed;

    private Vector3 moveInputs;
    private Vector3 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = MasterList["Move"];
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        float maxSpeedChange = accel * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, moveInputs.x, maxSpeedChange);
        velocity.y = Mathf.MoveTowards(velocity.y, moveInputs.y, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, moveInputs.z, maxSpeedChange);

        rb.linearVelocity = velocity;
    }

    private void Update()
    {
        moveDirection = CalcDirection();

        moveInputs = Move();
    }

    private Vector3 Move()
    {
        Vector2 move = movement.ReadValue<Vector2>();

        Vector3 moveDir = transform.right * move.x + transform.forward * move.y;

        moveDir = moveDir.normalized;

        moveDir = new Vector3(moveDir.x, 0, moveDir.z) * maxSpeed;

        return moveDir;
    }

    private Vector3 CalcDirection()
    {
        Vector2 move = movement.ReadValue<Vector2>();

        return transform.right * move.x + transform.forward * move.y;
    }
}
