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
    [SerializeField, Tooltip("This will add lerping to the camera.")] private bool smoothCamera = true;
    [SerializeField, Range(20f, 90f), Tooltip("90 = Smooth, 20 = SILK")] private float smoothness = 35f;

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
        if (!CanLookAround) return;

        if (smoothCamera) CalculateSmoothCameraRotation();
        else CalculateCameraRotation();

        LimitCamera();
        UpdateCameraRotation();
    }

    private void CalculateCameraRotation()
    {
        Vector2 mouseDelta = playerInputs.LookInput;

        mouseDelta.y = invertY ? mouseDelta.y * -1 : mouseDelta.y;

        yaw += mouseDelta.x;
        pitch -= mouseDelta.y;
    }

    private void CalculateSmoothCameraRotation()
    {
        Vector2 mouseDelta = playerInputs.LookInput;

        mouseDelta.y = invertY ? mouseDelta.y * -1 : mouseDelta.y;

        targetYaw += mouseDelta.x;
        targetPitch -= mouseDelta.y;

        yaw = Mathf.Lerp(yaw, targetYaw, smoothness * Time.deltaTime);
        pitch = Mathf.Lerp(pitch, targetPitch, smoothness * Time.deltaTime);
    }

    private void LimitCamera()
    {
        if (targetPitch > maxLookUpDownAndle)
        {
            targetPitch = Mathf.Lerp(pitch, maxLookUpDownAndle, .5f * Time.deltaTime);
        }
        else if (targetPitch < -maxLookUpDownAndle)
        {
            targetPitch = Mathf.Lerp(pitch, -maxLookUpDownAndle, .5f * Time.deltaTime);
        }

        pitch = Mathf.Clamp(pitch, -85f, 85f);
        targetPitch = Mathf.Clamp(targetPitch, -85f, 85f);
    }
    private void UpdateCameraRotation()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.y = yaw;
        transform.localRotation = Quaternion.Euler(rot);

        rot = cameraPivot.transform.localRotation.eulerAngles;
        rot.x = pitch;
        cameraPivot.transform.localRotation = Quaternion.Euler(rot);
    }
    
    public void SetCanLookAround(bool b)
    {
        CanLookAround = b;
    }
}
