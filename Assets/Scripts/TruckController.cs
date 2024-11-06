using UnityEngine;

public class TruckController : MonoBehaviour
{
    [Header("Game Management")]
    public GameManager gameManager; // Reference to the game manager

    [Header("References")]
    private Rigidbody rb; // Rigidbody component for physics calculations

    [Header("Driving Parameters")]
    public float accelerationForce = 800f; // Force applied for acceleration
    public float maxSpeed = 1500f; // Maximum forward speed
    public float reverseSpeed = 400f; // Maximum reverse speed
    public float turnSpeed = 200f; // Turning speed
    public float brakingForce = 400f; // Force applied for braking
    public float decelerationFactor = 2f; // Deceleration rate when no input is applied

    [Header("Physics Settings")]
    public float downforce = 40f; // Additional force to keep the truck on the ground
    public float grip = 5f; // Lateral grip to prevent sliding

    [Header("Input Sensitivity")]
    public float steerSensitivity = 0.6f; // Sensitivity for steering input
    public float accelerationSensitivity = 0.7f; // Sensitivity for acceleration input

    [Header("Ground Check")]
    public LayerMask groundLayer; // Layer used to check if the truck is grounded
    public float groundCheckDistance = 1f; // Distance used for ground check raycast

    private float moveInput; // Input for movement (forward/backward)
    private float steerInput; // Input for steering (left/right)
    private float currentSpeed; // Current speed of the truck
    public bool isGrounded; // Flag to check if the truck is on the ground

    private RaycastSensor raycastSensor; // Sensor component for raycasting
    public NeuralNetController neuralNetController; // Neural network controller

    void Start()
    {
        // Initialize Rigidbody and RaycastSensor components
        rb = GetComponent<Rigidbody>();
        raycastSensor = GetComponent<RaycastSensor>();

        // Set physical properties for stability
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        rb.mass = 15f;
        rb.drag = 0.1f;
        rb.angularDrag = 2f;
    }

    void Update()
    {
        if (gameManager.playable)
        {
            // Manual player control
            float targetMoveInput = Input.GetAxis("Vertical");
            moveInput = Mathf.Lerp(moveInput, targetMoveInput, Time.deltaTime * accelerationSensitivity);

            float targetSteerInput = Input.GetAxis("Horizontal");
            steerInput = Mathf.Lerp(steerInput, targetSteerInput, Time.deltaTime * steerSensitivity);
        }
        else
        {
            // Neural network control
            int actionIndex = neuralNetController.GetActionIndex();
            ExecuteAction(actionIndex);
            //Debug.Log($"[NeuralNet] Chosen Action Index: {actionIndex}");
        }
    }

    void FixedUpdate()
    {
        // Apply ground check and downforce
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
        // Check if the truck is grounded by casting a ray downward
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    void ApplyDownforce()
    {
        // Apply a downforce proportional to the truck's speed to improve stability
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
    }

    void HandleMotor()
    {
        // Calculate target speed based on input and direction
        float targetSpeed = moveInput * (moveInput > 0 ? maxSpeed : reverseSpeed);
        currentSpeed = rb.velocity.magnitude;

        Vector3 force = transform.forward * moveInput * accelerationForce;

        // Apply forward/reverse force if within speed limit
        if (currentSpeed < maxSpeed || (moveInput < 0 && currentSpeed < reverseSpeed))
        {
            rb.AddForce(force);
        }

        // Apply deceleration when no input is detected
        if (Mathf.Abs(moveInput) < 0.1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * decelerationFactor);
        }
    }

    void HandleSteering()
    {
        // Apply steering based on input and current speed
        if (currentSpeed > 0.1f)
        {
            float turnAngle = steerInput * turnSpeed * (rb.velocity.magnitude / maxSpeed);
            turnAngle = Mathf.Clamp(turnAngle, -turnSpeed, turnSpeed);

            Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    void ApplyGrip()
    {
        // Calculate lateral grip to prevent excessive sliding
        Vector3 sidewaysVelocity = Vector3.Dot(rb.velocity, transform.right) * transform.right;
        Vector3 correctedVelocity = rb.velocity - sidewaysVelocity * grip * Time.deltaTime;
        rb.velocity = correctedVelocity;
    }

    void ApplyAirResistance()
    {
        // Apply air resistance when airborne to gradually slow down the truck
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * decelerationFactor * 0.1f);
    }

    void LimitSpeed()
    {
        // Cap the speed to prevent exceeding maxSpeed
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(flatVelocity.x, rb.velocity.y, flatVelocity.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with objects tagged "Border"
        if (collision.gameObject.CompareTag("Border"))
        {
            gameManager.HandleBorderCollision();
        }
    }

    public void ExecuteAction(int actionIndex)
    {
        // Reset inputs
        moveInput = 0f;
        steerInput = 0f;

        // Execute specific action based on action index
        switch (actionIndex)
        {
            case 0: // Move forward
                moveInput = 1f;
                break;
            case 1: // Move forward and turn right
                moveInput = 1f;
                steerInput = 1f;
                break;
            case 2: // Move forward and turn left
                moveInput = 1f;
                steerInput = -1f;
                break;
            default:
                moveInput = 0f;
                steerInput = 0f;
                break;
        }
    }

    // Method to get the current speed of the truck
    public float GetSpeed()
    {
        return currentSpeed;
    }

    // Method to get the raycast sensor data
    public float[] GetRaycastData()
    {
        return raycastSensor.GetRaycastData();
    }

    // Method to reset movement inputs
    public void ResetInputs()
    {
        moveInput = 0f;
        steerInput = 0f;
    }
}
