using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxForwardSpeed = 2.5f;
    [SerializeField] private float maxBackwardSpeed = 1.5f;
    [SerializeField] private float maxStrafeSpeed = 2;
    [Space]
    [SerializeField] private float accForwardSpeed = 12.5f;
    [SerializeField] private float accBackwardSpeed = 7f;
    [SerializeField] private float accStrafeSpeed = 10f;
    [Space]
    [SerializeField] private float decelerateSpeed = 12.5f;

    public bool CanMove { get; private set; } = true;

    private Rigidbody rb;
    private PlayerInputs playerInputs;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }


    private void FixedUpdate()
    {
        if (CanMove) UpdateVelocity();
        UpdateDeceleration();
    }

    /// <summary>
    /// Updates the velocity of the object based on player input for movement.
    /// This method calculates the direction and speed of movement, caps the velocity
    /// based on predefined maximum speeds, and applies the final calculated velocity 
    /// to the Rigidbody component.
    /// </summary>
    private void UpdateVelocity()
    {
        // Get the movement input from the player
        Vector2 movementInput = playerInputs.MoveInput;
        Vector3 dir = Vector3.zero;

        // Calculate forward/backward movement
        dir += transform.forward * movementInput.y *
               (movementInput.y > 0 ? accForwardSpeed * Time.fixedDeltaTime : accBackwardSpeed * Time.fixedDeltaTime);

        // Calculate strafe movement (left/right)
        dir += transform.right * movementInput.x * accStrafeSpeed * Time.fixedDeltaTime;

        // Get the current linear velocity of the Rigidbody
        Vector3 velocity = rb.linearVelocity;

        // Update the velocity based on the movement direction
        velocity += dir;

        // Cap velocity based on movement direction
        if (movementInput.y > 0)  // Moving forward
        {
            if (velocity.magnitude > maxForwardSpeed)
            {
                velocity = velocity.normalized * maxForwardSpeed; // Normalize to max forward speed
            }
        }
        else if (movementInput.y < 0)  // Moving backward
        {
            if (velocity.magnitude > maxBackwardSpeed)
            {
                velocity = velocity.normalized * maxBackwardSpeed; // Normalize to max backward speed
            }
        }

        // Limit strafe velocity based on the movement direction
        if (movementInput.x != 0 && movementInput.y <= 0)
        {
            if (velocity.magnitude > maxStrafeSpeed)
            {
                velocity = velocity.normalized * maxStrafeSpeed; // Normalize to max strafe speed
            }
        }
        else if (movementInput.x != 0 && movementInput.y > 0)
        {
            if (velocity.magnitude > (maxStrafeSpeed + maxForwardSpeed) / 2)
            {
                velocity = velocity.normalized * maxStrafeSpeed; // Adjust strafe speed while moving forward
            }
        }

        // Apply the final calculated velocity to the Rigidbody
        rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Updates the deceleration of the object when no movement input is detected.
    /// This method gradually reduces the velocity of the object to create a realistic
    /// stopping effect. It ensures that velocity does not reverse direction when 
    /// transitioning to a stop.
    /// </summary>
    private void UpdateDeceleration()
    {
        // Get the movement input from the player
        Vector2 moveInput = playerInputs.MoveInput;

        // Check if there is no movement input
        if (Mathf.Abs(moveInput.x) <= 1e-5 && Mathf.Abs(moveInput.y) <= 1e-5)
        {
            // Get the current linear velocity of the Rigidbody
            Vector3 velocity = rb.linearVelocity;

            // Gradually reduce the velocity to create a deceleration effect
            velocity -= velocity.normalized * decelerateSpeed * Time.fixedDeltaTime;

            // Ensure that the velocity does not reverse direction
            if ((velocity.x < 0 && rb.linearVelocity.x > 0) || (velocity.x > 0 && rb.linearVelocity.x < 0))
            {
                velocity.x = 0;
            }
            if ((velocity.y < 0 && rb.linearVelocity.y > 0) || (velocity.y > 0 && rb.linearVelocity.y < 0))
            {
                velocity.y = 0;
            }
            if ((velocity.z < 0 && rb.linearVelocity.z > 0) || (velocity.z > 0 && rb.linearVelocity.z < 0))
            {
                velocity.z = 0;
            }

            // Apply the updated velocity back to the Rigidbody
            rb.linearVelocity = velocity;
        }
    }
    public void SetCanMove(bool b)
    {
        CanMove = b;
    }
}
