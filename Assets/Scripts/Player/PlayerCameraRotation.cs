using UnityEngine;

/// <summary>
/// Handles the camera rotation based on player input, including smooth transitions and limiting the pitch angle.
/// Allows for toggling whether the player can look around.
/// </summary>
/// <remarks>
/// <b>Key Features:</b>
/// <list type="bullet">
/// <item><description><c>invertY</c>: Option to invert the vertical camera movement.</description></item>
/// <item><description><c>smoothCamera</c>: Determines whether the camera rotation should be smooth or instant.</description></item>
/// <item><description><c>maxLookUpDownAndle</c>: Limits how far the player can look up or down.</description></item>
/// <item><description><c>CanLookAround</c>: A flag to toggle whether the player can rotate the camera or not.</description></item>
/// <item><description><c>SetCanLookAround()</c>: Method to set the <c>CanLookAround</c> property dynamically.</description></item>
/// </list>
/// </remarks>
public class PlayerCameraRotation : MonoBehaviour
{
    private PlayerInputs playerInputs;

    [Header("Camera")]
    [SerializeField]
    private Transform cameraPivot; // Reference to the camera pivot that controls vertical rotation.

    [Space]

    [Tooltip("Invert the vertical camera movement.")]
    [SerializeField]
    private bool invertY = false; // Inverts the Y-axis for camera control.

    [SerializeField, Range(60, 85), Tooltip("Maximum pitch angle the camera can rotate up or down.")]
    private float maxLookUpDownAndle = 75f; // Max limit for vertical camera rotation.

    [Space]

    [Tooltip("This will add lerping to the camera rotation for smooth transitions.")]
    [SerializeField]
    private bool smoothCamera = true; // Determines if camera rotation is smooth.

    [SerializeField, Range(20f, 90f), Tooltip("Higher value results in smoother transitions. Lower is more responsive.")]
    private float smoothness = 35f; // Controls the smoothness of the camera rotation.

    private float targetYaw, targetPitch;

    private float yaw;
    private float pitch;

    public bool CanLookAround { get; private set; } = true; // Determines whether the player is allowed to rotate the camera.

    private void Start()
    {
        // Lock the cursor to the center of the screen for first-person view.
        Cursor.lockState = CursorLockMode.Locked;

        // Find the PlayerInputs object for handling input.
        playerInputs = FindFirstObjectByType<PlayerInputs>();

        // Check if PlayerInputs was found, if not instantiate it.
        if (playerInputs == null)
        {
            Debug.LogWarning($"{nameof(PlayerCameraRotation)} could not find a {nameof(PlayerInputs)} in the scene. Instantiating a new one.");
            playerInputs = Instantiate(new PlayerInputs());
        }
    }



    private void Update()
    {
        // If the player cannot look around, exit the method.
        if (!CanLookAround) return;

        // Calculate the camera rotation based on whether smoothing is enabled.
        if (smoothCamera) CalculateSmoothCameraRotation();
        else CalculateCameraRotation();

        // Limit the camera rotation to prevent it from going too far up or down.
        LimitCamera();

        // Update the camera rotation based on calculated yaw and pitch.
        UpdateCameraRotation();
    }

    /// <summary>
    /// Calculates the camera rotation without smoothing.
    /// </summary>
    private void CalculateCameraRotation()
    {
        // Get the mouse movement input for looking around.
        Vector2 mouseDelta = playerInputs.LookInput;

        // Apply Y-axis inversion if set.
        mouseDelta.y = invertY ? mouseDelta.y * -1 : mouseDelta.y;

        // Adjust yaw and pitch based on the mouse input.
        yaw += mouseDelta.x;
        pitch -= mouseDelta.y;
    }

    /// <summary>
    /// Calculates the camera rotation with smooth interpolation between target angles.
    /// </summary>
    private void CalculateSmoothCameraRotation()
    {
        // Get the mouse movement input for looking around.
        Vector2 mouseDelta = playerInputs.LookInput;

        // Apply Y-axis inversion if set.
        mouseDelta.y = invertY ? mouseDelta.y * -1 : mouseDelta.y;

        // Update target yaw and pitch based on mouse input.
        targetYaw += mouseDelta.x;
        targetPitch -= mouseDelta.y;

        // Smoothly interpolate yaw and pitch values.
        yaw = Mathf.Lerp(yaw, targetYaw, smoothness * Time.deltaTime);
        pitch = Mathf.Lerp(pitch, targetPitch, smoothness * Time.deltaTime);
    }

    /// <summary>
    /// Limits the camera pitch to prevent the camera from rotating too far up or down.
    /// </summary>
    private void LimitCamera()
    {
        // Clamp the target pitch to prevent exceeding max vertical limits.
        if (targetPitch > maxLookUpDownAndle)
        {
            targetPitch = Mathf.Lerp(pitch, maxLookUpDownAndle, .5f * Time.deltaTime);
        }
        else if (targetPitch < -maxLookUpDownAndle)
        {
            targetPitch = Mathf.Lerp(pitch, -maxLookUpDownAndle, .5f * Time.deltaTime);
        }

        // Clamp pitch within the limits.
        pitch = Mathf.Clamp(pitch, -85f, 85f);
        targetPitch = Mathf.Clamp(targetPitch, -85f, 85f);
    }

    /// <summary>
    /// Updates the camera's rotation based on the calculated yaw and pitch values.
    /// </summary>
    private void UpdateCameraRotation()
    {
        // Apply yaw rotation to the player.
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.y = yaw;
        transform.localRotation = Quaternion.Euler(rot);

        // Apply pitch rotation to the camera pivot (up/down movement).
        rot = cameraPivot.transform.localRotation.eulerAngles;
        rot.x = pitch;
        cameraPivot.transform.localRotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// Sets whether the player can look around or not.
    /// </summary>
    /// <param name="b">Set to true to allow camera rotation, false to disable it.</param>
    public void SetCanLookAround(bool b)
    {
        CanLookAround = b;
    }
}
