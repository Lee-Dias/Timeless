using UnityEditor.Rendering;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    private PlayerInputs playerInputs;

    [Header("Camera")]
    [SerializeField] Transform cameraPivot;
    [Space]
    [SerializeField] private bool invertY = false;
    [SerializeField, Range(60, 85)] private float maxLookUpDownAndle = 75;
    [Space]
    [SerializeField, Tooltip("Rotation multiplication for mouse")] private float mouseSensitivity = 50;
    [SerializeField, Tooltip("Rotation multiplication for gamepad")] private float gamepadSensitivity = 80f;
    [Space]
    [SerializeField, Tooltip("This will add lerping to the camera.")] private bool smoothCamera = true;
    [SerializeField, Range(20f, 90f), Tooltip("90 = Smooth, 20 = SILK")] private float smoothness = 35f;


    private float sensitivity;
    private float targetYaw, targetPitch;

    private float yaw;
    private float pitch;

    public bool CanLookAround { get; private set; } = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    private void Update()
    {
        sensitivity = playerInputs.currentDeviceType == PlayerInputs.DeviceType.KeyboardMouse ? mouseSensitivity : gamepadSensitivity;

        UpdateRotation();
    }

    /// <summary>
    /// Updates the rotation of the camera based on player input.
    /// This method handles both the yaw (horizontal rotation) and pitch (vertical rotation) of the camera,
    /// optionally applying smoothing and inverting the Y-axis input.
    /// </summary>
    private void UpdateRotation()
    {
        // Get the change in mouse position since the last frame
        Vector2 mouseDelta = playerInputs.LookInput;

        // Invert the Y-axis if specified
        mouseDelta.y = invertY ? mouseDelta.y * -1 : mouseDelta.y;

        // Update target yaw and pitch based on mouse movement and sensitivity
        targetYaw += mouseDelta.x * sensitivity * Time.deltaTime;
        targetPitch -= mouseDelta.y * sensitivity * Time.deltaTime;

        // Apply smoothing to the yaw and pitch if enabled
        if (smoothCamera)
        {
            yaw = Mathf.Lerp(yaw, targetYaw, smoothness * Time.deltaTime);
            pitch = Mathf.Lerp(pitch, targetPitch, smoothness * Time.deltaTime);
        }
        else
        {
            // If smoothing is disabled, set yaw and pitch directly to target values
            yaw = targetYaw;
            pitch = targetPitch;
        }

        // Clamp the target pitch to prevent excessive upward or downward looking
        if (targetPitch > maxLookUpDownAndle)
        {
            targetPitch = Mathf.Lerp(pitch, maxLookUpDownAndle, .5f * Time.deltaTime);
        }
        else if (targetPitch < -maxLookUpDownAndle)
        {
            targetPitch = Mathf.Lerp(pitch, -maxLookUpDownAndle, .5f * Time.deltaTime);
        }

        // Clamp pitch values to prevent unrealistic rotations
        pitch = Mathf.Clamp(pitch, -85f, 85f);
        targetPitch = Mathf.Clamp(targetPitch, -85f, 85f);

        // Update the camera's yaw rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.y = yaw;
        transform.localRotation = Quaternion.Euler(rot);

        // Update the camera pivot's pitch rotation
        rot = cameraPivot.transform.localRotation.eulerAngles;
        rot.x = pitch;
        cameraPivot.transform.localRotation = Quaternion.Euler(rot);
    }


    public void SetCanLookAround(bool b)
    {
        CanLookAround = b;
    }
}
