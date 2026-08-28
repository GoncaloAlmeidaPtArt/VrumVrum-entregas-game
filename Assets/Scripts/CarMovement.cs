using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Basic arcade-style car controller.
/// Attach this to your car GameObject (the root, with a Rigidbody on it).
/// Works with a simple box/capsule collider — no wheel colliders needed,
/// which makes it a good starting point before moving to a full physics rig.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 18f;          // top forward speed (units/sec)
    [SerializeField] private float maxReverseSpeed = 8f;    // top reverse speed
    [SerializeField] private float acceleration = 14f;      // how fast speed builds up
    [SerializeField] private float braking = 22f;           // how fast speed drops when reversing/braking
    [SerializeField] private float friction = 6f;           // natural deceleration with no input
    [SerializeField] private float turnSpeed = 120f;        // degrees/sec at full speed
    [SerializeField] private string moveInputName = "Move";

    private Rigidbody rb;
    private float currentSpeed = 0f;      // current forward/back speed
    private float steerInput = 0f;
    private float throttleInput = 0f;
    private InputAction input;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Lower the center of mass a bit so the car doesn't tip over easily
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        input = InputSystem.actions[moveInputName];
    }

    void Update()
    {
        // Read input every frame (works with arrow keys or WASD by default
        // via Unity's built-in "Horizontal"/"Vertical" axes)
        throttleInput = -input.ReadValue<Vector2>().y;   // W/S or Up/Down
        steerInput = -input.ReadValue<Vector2>().x;    // A/D or Left/Right
    }

    void FixedUpdate()
    {
        HandleAcceleration();
        HandleSteering();
        ApplyMovement();
    }

    private void HandleAcceleration()
    {
        if (throttleInput > 0.01f)
        {
            currentSpeed += acceleration * throttleInput * Time.fixedDeltaTime;
        }
        else if (throttleInput < -0.01f)
        {
            currentSpeed += braking * throttleInput * Time.fixedDeltaTime;
        }
        else
        {
            // No input: friction pulls speed back toward zero
            if (currentSpeed > 0f)
                currentSpeed = Mathf.Max(0f, currentSpeed - friction * Time.fixedDeltaTime);
            else if (currentSpeed < 0f)
                currentSpeed = Mathf.Min(0f, currentSpeed + friction * Time.fixedDeltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxReverseSpeed, maxSpeed);
    }

    private void HandleSteering()
    {
        // Only steer while actually moving, and flip steering when reversing
        // (matches how a real car behaves when backing up)
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);
            float steerDir = currentSpeed >= 0f ? 1f : -1f;
            float turnAmount = steerInput * turnSpeed * speedFactor * steerDir * Time.fixedDeltaTime;

            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    private void ApplyMovement()
    {
        // Move along the car's own forward direction at currentSpeed
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMovement);
    }
}