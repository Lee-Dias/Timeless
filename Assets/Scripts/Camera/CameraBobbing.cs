using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a stylized camera bobbing effect to simulate natural movement during walking.
/// </summary>
/// <remarks>
/// This class enhances player immersion by applying sinusoidal vertical and horizontal 
/// motion to the camera. The bobbing effect is dynamic, activating only when the player's 
/// movement exceeds a speed threshold. If the player stops, the camera smoothly returns 
/// to its original position. Also keeps the camera focused on a target point in front of the player.
/// 
/// <para><b>Dependencies:</b></para>
/// <list type="bullet">
/// <item>
/// <description>A Rigidbody component on the same GameObject to track velocity.</description>
/// </item>
/// <item>
/// <description>A Transform reference for the camera bobbing object (<c>cameraBobber</c>).</description>
/// </item>
/// <item>
/// <description>A Transform reference for the pivot point of the camera (<c>cameraPivot</c>).</description>
/// </item>
/// </list>
/// </remarks>
[RequireComponent(typeof(Rigidbody))]
public class CameraBobbing : MonoBehaviour
{
    [Header("Configuration")]
    public bool enable = true;

    [Tooltip("Amplitude of the bobbing effect (vertical and horizontal movement).")]
    [SerializeField, Range(0, 0.1f)]
    private float amplitude = 0.00065f;

    [Tooltip("Frequency of the bobbing effect (speed of movement).")]
    [SerializeField, Range(0, 30)]
    private float frequency = 8.0f;

    [Tooltip("Reference to the transform of the camera bobbing object.")]
    [SerializeField]
    private Transform cameraBobber;

    [Tooltip("Reference to the transform of the camera pivot point.")]
    [SerializeField]
    private Transform cameraPivot;

    // Speed threshold at which bobbing starts
    private float toggleSpeed = 1.5f;

    // Initial position of the camera bobbing object
    private Vector3 startPos;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Debug.Assert(cameraBobber != null, $"{nameof(cameraBobber)} is not assigned. Please assign it in the Inspector.");
        Debug.Assert(cameraPivot != null, $"{nameof(cameraPivot)} is not assigned. Please assign it in the Inspector.");

        if (cameraBobber != null)
        {
            startPos = cameraBobber.localPosition;
        }
    }
    private void OnValidate()
    {
        if (cameraBobber == null)
        {
            Debug.LogWarning($"{nameof(cameraBobber)} is missing, please assign it in the Inspector.");
        }

        if (cameraPivot == null)
        {
            Debug.LogWarning($"{nameof(cameraPivot)} is missing, please assign it in the Inspector.");
        }
    }


    // Updates the camera bobbing effect on each fixed frame.
    // Checks motion to trigger bobbing, resets position if required,
    // and ensures the camera looks at the target.
    private void Update()
    {
        if (!enable || rb == null || cameraBobber == null || cameraPivot == null) return;

        if (rb.linearVelocity.magnitude > 1e-5)
        {
            CheckMotion();
        }
        else ResetPosition();

        cameraBobber.LookAt(FocusTarget());
    }


    /// <summary>
    /// Calculates the bobbing motion to simulate footsteps.
    /// </summary>
    /// <returns> Returns a Vector3 representing the bobbing position adjustment. </returns>
    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude * 2;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude;
        return pos;
    }

    /// <summary>
    /// Checks if the player is moving fast enough to trigger the bobbing effect.
    /// </summary>
    private void CheckMotion()
    {
        float speed = rb.linearVelocity.magnitude;

        // Only apply bobbing if speed exceeds the toggle threshold
        if (speed < toggleSpeed) return;

        PlayMotion(FootStepMotion());
    }

    /// <summary>
    /// Applies the calculated bobbing motion to the camera position.
    /// </summary>
    /// <param name="motion">The calculated bobbing motion Vector3.</param>
    private void PlayMotion(Vector3 motion)
    {
        cameraBobber.localPosition += motion;
    }

    /// <summary>
    /// <br>Determines the target focus point for the camera to look at.</br>
    /// <br>Uses the camera pivot as a reference for position.</br>
    /// </summary>
    /// <returns>
    /// Returns a Vector3 representing the focus point in front of the player.
    /// </returns>
    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraPivot.localPosition.y, transform.position.z);
        pos += cameraPivot.forward * 15.0f;
        return pos;
    }

    /// <summary>
    /// <br>Resets the camera position smoothly back to the starting position.</br>
    /// <br>This maintains a stable base when bobbing is disabled or motion stops.</br>
    /// </summary>
    private void ResetPosition()
    {
        if (cameraBobber.localPosition == startPos) return;

        cameraBobber.localPosition = Vector3.Lerp(cameraBobber.localPosition, startPos, 10 * Time.deltaTime);
    }

    public void SetEnabled(bool b)
    {
        enabled = b;
    }
}
