using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Provides a dynamic camera sway effect to simulate subtle rotational motion based on player input.
/// </summary>
/// <remarks>
/// This class enhances immersion by applying a rotation effect to the camera
/// in response to mouse movement. The sway effect adds realism by 
/// mimicking the natural tilt that occurs when turning or looking around.
/// 
/// <para><b>Key Features:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Applies a smooth sway effect based on mouse input.</description>
/// </item>
/// <item>
/// <description>Customizable sensitivity and smoothing to tailor the effect.</description>
/// </item>
/// <item>
/// <description>Clamp angles to prevent excessive or unnatural rotations.</description>
/// </item>
/// <item>
/// <description>Enable/disable functionality for toggling the effect dynamically.</description>
/// </item>
/// </list>
/// 
/// <para><b>How It Works:</b></para>
/// The script captures the player's mouse movement and calculates a rotational offset 
/// for the specified <c>swayObject</c>. This offset is interpolated over time using 
/// a smoothing factor to create fluid transitions. The rotation is clamped between 
/// configurable minimum and maximum angles to maintain realism.
/// 
/// <para><b>Dependencies:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Requires a <c>PlayerInputs</c> script to provide mouse input data.</description>
/// </item>
/// <item>
/// <description>Requires a reference to the <c>Transform</c> that will be rotated (<c>swayObject</c>).</description>
/// </item>
/// </list>
/// 
/// </remarks>

public class CameraSway : MonoBehaviour
{
    public bool enable = true;

    [SerializeField, Tooltip("Object that will be applied the effect")]
    private Transform swayObject;

    [SerializeField, Tooltip("Sway effect multiplication")]
    private float sensibility = 1;
    [SerializeField] private bool smoothSway = true;

    [SerializeField, Range(0, 10), Tooltip("Default: 7"), ShowIf(nameof(smoothSway))]
    private float swaySmoothness = 7f;

    [Space]

    [SerializeField, Tooltip("The min angle it will rotate")]
    private float clampMin = -7.5f;
    [SerializeField, Tooltip("The max angle it will rotate")]
    private float clampMax = 7.5f;

    PlayerInputs playerInputs;

    private float turn = 0f;

    private void Start()
    {
        // Try to find the PlayerInputs object in the scene
        playerInputs = FindFirstObjectByType<PlayerInputs>();

        // Check if playerInputs was found, and handle accordingly
        if (playerInputs == null)
        {
            // Log a warning if it's not found, then instantiate a new one
            Debug.LogWarning($"{nameof(CameraSway)} requires a {nameof(PlayerInputs)} object in the scene. Instantiating a new one.");

            // Instantiate the PlayerInputs if not found
            playerInputs = Instantiate(new PlayerInputs());
        }
    }


    /// <summary>
    /// <br>Will rotate the <see cref="swayObject"/> based on mouse input.</br>
    /// </summary>
    void Update()
    {
        if (!enable) return;

        Vector2 mouseDelta = playerInputs.LookInput;

        if (smoothSway) turn = Mathf.Lerp(turn, Mathf.Clamp(mouseDelta.x * sensibility, clampMin, clampMax), swaySmoothness * Time.deltaTime);
        else turn = Mathf.Clamp(mouseDelta.x * sensibility, clampMin, clampMax);

        Vector3 rot = swayObject.localRotation.eulerAngles;
        rot.z = turn;
        swayObject.localRotation = Quaternion.Euler(rot);
    }

    public void SetEnabled(bool b)
    {
        enabled = b;
    }
}
