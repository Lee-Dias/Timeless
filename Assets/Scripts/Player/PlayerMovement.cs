using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// Manages the movement of the player character, including forward/backward and strafing movement with acceleration and deceleration.
/// It also handles toggling the ability to move, and provides debugging functionality for velocity updates and movement changes.
///
/// The player movement is controlled by the following inputs:
/// <list type="bullet">
///     <item><description>W key / S key: Move the player forward and backward with respective speeds.</description></item>
///     <item><description>A key / D key: Move the player left and right (strafe).</description></item>
/// </list>
/// 
/// <para>
/// The movement system uses acceleration and deceleration to smoothly transition between states:
/// <list type="bullet">
///     <item><description>Acceleration: Applies a specified forward, backward, or strafing acceleration when input is detected.</description></item>
///     <item><description>Deceleration: When no input is detected, the velocity gradually decreases to simulate deceleration.</description></item>
/// </list>
/// </para>
///
/// The following flags and events are provided:
/// <list type="bullet">
///     <item><description>CanMove (public property): Determines if the player is allowed to move. This can be toggled by external components.</description></item>
///     <item><description>showDebugMessages (serialized field): Flag to enable or disable debug messages for velocity and movement changes.</description></item>
/// </list>
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    /// <summary>
    /// The maximum forward speed of the player.
    /// </summary>
    [SerializeField] private float maxForwardSpeed = 2.5f;

    /// <summary>
    /// The maximum backward speed of the player.
    /// </summary>
    [SerializeField] private float maxBackwardSpeed = 1.5f;

    /// <summary>
    /// The maximum strafing speed of the player.
    /// </summary>
    [SerializeField] private float maxStrafeSpeed = 2;

    [Space]

    /// <summary>
    /// The forward acceleration of the player.
    /// </summary>
    [SerializeField] private float accForwardSpeed = 12.5f;

    /// <summary>
    /// The backward acceleration of the player.
    /// </summary>
    [SerializeField] private float accBackwardSpeed = 7f;

    /// <summary>
    /// The strafing acceleration of the player.
    /// </summary>
    [SerializeField] private float accStrafeSpeed = 10f;

    [Space]

    /// <summary>
    /// The deceleration rate when the player stops providing movement input.
    /// </summary>
    [SerializeField] private float decelerateSpeed = 12.5f;

    [Header("Debugging")]

    /// <summary>
    /// Flag to enable or disable debug messages from this script.
    /// </summary>
    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    /// <summary>
    /// Flag to include the object's name in debug messages if enabled.
    /// </summary>
    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;

    /// <summary>
    /// Property to check if the player can currently move.
    /// </summary>
    public bool CanMove { get; private set; } = true;

    private Rigidbody rb;  // Rigidbody component of the player.
    private PlayerInputs playerInputs;  // PlayerInputs instance to handle input.

    private void Awake()
    {
        // Initialize Rigidbody and disable gravity.
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        Log("Rigidbody initialized and gravity disabled.");
    }

    private void Start()
    {
        // Lock the cursor and attempt to find PlayerInputs in the scene.
        Cursor.lockState = CursorLockMode.Locked;

        playerInputs = FindFirstObjectByType<PlayerInputs>();

        if (playerInputs == null)
        {
            Debug.LogWarning("PlayerInputs component not found in the scene! Instantiating one...");
            playerInputs = Instantiate(new PlayerInputs());
        }

        Log("Player movement initialized successfully.");
    }

    private void FixedUpdate()
    {
        // Update velocity if movement is allowed, then handle deceleration.
        if (CanMove)
        {
            UpdateVelocity();
        }
        UpdateDeceleration();
    }

    /// <summary>
    /// Updates the velocity of the player based on the player's input for movement.
    /// </summary>
    private void UpdateVelocity()
    {
        Vector2 movementInput = playerInputs.MoveInput;  // Get input for movement.
        Vector3 dir = Vector3.zero;

        // Update movement direction based on input (forward, backward, strafing).
        dir += transform.forward * movementInput.y *
               (movementInput.y > 0 ? accForwardSpeed * Time.fixedDeltaTime : accBackwardSpeed * Time.fixedDeltaTime);
        dir += transform.right * movementInput.x * accStrafeSpeed * Time.fixedDeltaTime;

        // Get current velocity and apply direction changes.
        Vector3 velocity = rb.linearVelocity;
        velocity += dir;

        // Clamp velocity to max speeds.
        if (movementInput.y > 0 && velocity.magnitude > maxForwardSpeed)
        {
            velocity = velocity.normalized * maxForwardSpeed;
        }
        else if (movementInput.y < 0 && velocity.magnitude > maxBackwardSpeed)
        {
            velocity = velocity.normalized * maxBackwardSpeed;
        }
        if (movementInput.x != 0 && velocity.magnitude > maxStrafeSpeed)
        {
            velocity = velocity.normalized * maxStrafeSpeed;
        }

        // Apply updated velocity to the Rigidbody.
        rb.linearVelocity = velocity;
        Log($"Updated velocity: {velocity}");
    }

    /// <summary>
    /// Gradually reduces the velocity to simulate deceleration when no movement input is detected.
    /// </summary>
    private void UpdateDeceleration()
    {
        // Get the movement input from the player
        Vector2 moveInput = playerInputs.MoveInput;

        // Check if there is no movement input
        if ((Mathf.Abs(moveInput.x) <= 1e-5 && Mathf.Abs(moveInput.y) <= 1e-5) || !CanMove)
        {
            // Get the current linear velocity of the Rigidbody
            Vector3 velocity = rb.linearVelocity;

            // Gradually reduce the velocity to create a deceleration effect
            velocity -= velocity.normalized * decelerateSpeed * Time.fixedDeltaTime;

            // Ensure that the velocity does not reverse direction
            if ((velocity.x < 1e-5 && rb.linearVelocity.x > 1e-5) || (velocity.x > 1e-5 && rb.linearVelocity.x < 1e-5))
            {
                velocity.x = 0;
            }
            if ((velocity.y < 1e-5 && rb.linearVelocity.y > 1e-5) || (velocity.y > 1e-5 && rb.linearVelocity.y < 1e-5))
            {
                velocity.y = 0;
            }
            if ((velocity.z < 1e-5 && rb.linearVelocity.z > 1e-5) || (velocity.z > 1e-5 && rb.linearVelocity.z < 1e-5))
            {
                velocity.z = 0;
            }

            // Apply the updated velocity back to the Rigidbody
            rb.linearVelocity = velocity;
        }
    }

    /// <summary>
    /// Enables or disables the ability for the player to move.
    /// </summary>
    /// <param name="b">If true, enables movement; if false, disables movement.</param>
    public void SetCanMove(bool b)
    {
        CanMove = b;
        Log($"CanMove set to: {b}");
    }

    /// <summary>
    /// Logs a debug message to the Console if debugging is enabled.
    /// Includes the object's name as an identifier if 'identifyObject' is true.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    private void Log(string message)
    {
        if (showDebugMessages)
        {
            if (identifyObject)
                Debug.Log(message, this); // Includes object name in the log message.
            else
                Debug.Log(message); // Logs without object name.
        }
    }
}
