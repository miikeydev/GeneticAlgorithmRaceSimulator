using UnityEngine;

public class TruckController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;

    [Header("Driving Parameters")]
    public float accelerationForce = 800f; // Increased for better responsiveness
    public float maxSpeed = 1500f;
    public float reverseSpeed = 400f; // Separate reverse speed limit
    public float turnSpeed = 200f; // Adjusted for smoother turning
    public float brakingForce = 400f; // Increased braking force
    public float decelerationFactor = 2f; // For natural deceleration

    [Header("Physics Settings")]
    public float downforce = 40f; // Constant downforce
    public float grip = 5f; // Side grip factor

    [Header("Input Sensitivity")]
    public float steerSensitivity = 0.6f; // Smoother steering input
    public float accelerationSensitivity = 0.7f; // Smoother acceleration input

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1f;

    private float moveInput;
    private float steerInput;
    private float currentSpeed;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Lower center of mass
        rb.mass = 15f; // Typical mass for a truck
        rb.drag = 0.1f;
        rb.angularDrag = 2f; // Increased for better stability
    }

    void Update()
    {
        // Get player input with smoothing
        float targetMoveInput = Input.GetAxis("Vertical");
        moveInput = Mathf.Lerp(moveInput, targetMoveInput, Time.deltaTime * accelerationSensitivity);

        float targetSteerInput = Input.GetAxis("Horizontal");
        steerInput = Mathf.Lerp(steerInput, targetSteerInput, Time.deltaTime * steerSensitivity);
    }

    void FixedUpdate()
    {
        CheckGround();
        ApplyDownforce();

        if (isGrounded)
        {
            HandleMotor();
            HandleSteering();
            ApplyGrip();
        }
        else
        {
            ApplyAirResistance();
        }

        LimitSpeed();
    }

    void CheckGround()
    {
        // Raycast to check if the vehicle is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    void ApplyDownforce()
    {
        // Apply downforce to keep the vehicle grounded
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
    }

    void HandleMotor()
    {
        // Calculate the desired speed based on input
        float targetSpeed = moveInput * (moveInput > 0 ? maxSpeed : reverseSpeed);
        currentSpeed = rb.velocity.magnitude;

        // Calculate force to apply
        Vector3 force = transform.forward * moveInput * accelerationForce;

        // Apply force if under max speed
        if (currentSpeed < maxSpeed || moveInput < 0 && currentSpeed < reverseSpeed)
        {
            rb.AddForce(force);
        }

        // Apply natural deceleration when no input
        if (Mathf.Abs(moveInput) < 0.1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * decelerationFactor);
        }
    }

    void HandleSteering()
    {
        if (currentSpeed > 0.1f)
        {
            // Calculate turn angle based on speed
            float turnAngle = steerInput * turnSpeed * (rb.velocity.magnitude / maxSpeed);
            turnAngle = Mathf.Clamp(turnAngle, -turnSpeed, turnSpeed);

            // Rotate the vehicle
            Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    void ApplyGrip()
    {
        // Reduce sideways drift
        Vector3 sidewaysVelocity = Vector3.Dot(rb.velocity, transform.right) * transform.right;
        Vector3 correctedVelocity = rb.velocity - sidewaysVelocity * grip * Time.fixedDeltaTime;
        rb.velocity = correctedVelocity;
    }

    void ApplyAirResistance()
    {
        // Apply additional drag when airborne
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * decelerationFactor * 0.1f);
    }

    void LimitSpeed()
    {
        // Clamp the speed to the max speed
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(flatVelocity.x, rb.velocity.y, flatVelocity.z);
        }
    }
}
