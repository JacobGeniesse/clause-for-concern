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
        moveInputs = Move();
    }

    private Vector3 Move()
    {
        Vector2 move = movement.ReadValue<Vector2>();

        return new Vector3(move.x * maxSpeed, 0, move.y * maxSpeed);
    }
}
