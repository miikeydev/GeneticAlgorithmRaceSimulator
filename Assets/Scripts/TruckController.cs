using UnityEngine;

public class TruckController : MonoBehaviour
{
    [Header("Game Management")]
    public GameManager gameManager;

    [Header("References")]
    private Rigidbody rb;

    [Header("Driving Parameters")]
    public float accelerationForce = 800f;
    public float maxSpeed = 1500f;
    public float reverseSpeed = 400f;
    public float turnSpeed = 200f;
    public float brakingForce = 400f;
    public float decelerationFactor = 2f;

    [Header("Physics Settings")]
    public float downforce = 40f;
    public float grip = 5f;

    [Header("Input Sensitivity")]
    public float steerSensitivity = 0.6f;
    public float accelerationSensitivity = 0.7f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1f;

    private float moveInput;
    private float steerInput;
    private float currentSpeed;
    public bool isGrounded;

    private RaycastSensor raycastSensor;
    public NeuralNetController neuralNetController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        raycastSensor = GetComponent<RaycastSensor>(); 

        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        rb.mass = 15f;
        rb.drag = 0.1f;
        rb.angularDrag = 2f;
    }

    void Update()
    {
        if (gameManager.playable)
        {
            // Contrôle manuel par le joueur
            float targetMoveInput = Input.GetAxis("Vertical");
            moveInput = Mathf.Lerp(moveInput, targetMoveInput, Time.deltaTime * accelerationSensitivity);

            float targetSteerInput = Input.GetAxis("Horizontal");
            steerInput = Mathf.Lerp(steerInput, targetSteerInput, Time.deltaTime * steerSensitivity);
        }
        else
        {
            // Contrôle par le réseau de neurones
            int actionIndex = neuralNetController.GetActionIndex();
            ExecuteAction(actionIndex);
            Debug.Log($"[NeuralNet] Action Index choisi: {actionIndex}");
        }
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
    }

    void HandleMotor()
    {
        float targetSpeed = moveInput * (moveInput > 0 ? maxSpeed : reverseSpeed);
        currentSpeed = rb.velocity.magnitude;

        Vector3 force = transform.forward * moveInput * accelerationForce;

        if (currentSpeed < maxSpeed || (moveInput < 0 && currentSpeed < reverseSpeed))
        {
            rb.AddForce(force);
        }

        if (Mathf.Abs(moveInput) < 0.1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * decelerationFactor);
        }
    }

    void HandleSteering()
    {
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
        Vector3 sidewaysVelocity = Vector3.Dot(rb.velocity, transform.right) * transform.right;
        Vector3 correctedVelocity = rb.velocity - sidewaysVelocity * grip * Time.fixedDeltaTime;
        rb.velocity = correctedVelocity;
    }

    void ApplyAirResistance()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * decelerationFactor * 0.1f);
    }

    void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(flatVelocity.x, rb.velocity.y, flatVelocity.z);
        }
    }

    public void ExecuteAction(int actionIndex)
    {
        moveInput = 0f;
        steerInput = 0f;

        switch (actionIndex)
        {
            case 0: // Avancer
                moveInput = 1f;
                break;
            case 1: // Reculer
                moveInput = -1f;
                break;
            case 2: // Tourner à gauche (sans avancer/reculer)
                steerInput = -1f;
                break;
            case 3: // Tourner à droite (sans avancer/reculer)
                steerInput = 1f;
                break;
            case 4: // Avancer et tourner à gauche
                moveInput = 1f;
                steerInput = -1f;
                break;
            case 5: // Avancer et tourner à droite
                moveInput = 1f;
                steerInput = 1f;
                break;
            case 6: // Reculer et tourner à gauche
                moveInput = -1f;
                steerInput = -1f;
                break;
            case 7: // Reculer et tourner à droite
                moveInput = -1f;
                steerInput = 1f;
                break;
            default:
                moveInput = 0f;
                steerInput = 0f;
                break;
        }
    }

    // Méthode pour obtenir la vitesse actuelle
    public float GetSpeed()
    {
        return currentSpeed;
    }

    // Méthode pour obtenir les distances des raycasts
    public float[] GetRaycastData()
    {
        return raycastSensor.GetRaycastData();
    }
    public void ResetInputs()
    {
        moveInput = 0f;
        steerInput = 0f;
    }

}
