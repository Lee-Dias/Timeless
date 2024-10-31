using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    InteractionEventsHandler interactionEventsHandler;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = cameraBobber.localPosition;
    }
    private void Start()
    {
        interactionEventsHandler = FindFirstObjectByType<InteractionEventsHandler>();
        interactionEventsHandler.InspectObject += InspectObject;
        interactionEventsHandler.FinishInspect += FinishInspect;
    }

    // Updates the camera bobbing effect on each fixed frame.
    // Checks motion to trigger bobbing, resets position if required,
    // and ensures the camera looks at the target.
    private void Update()
    {
        if (!enable) return;

        CheckMotion();
        ResetPosition();
        cameraBobber.LookAt(FocusTarget());
    }

    /// <summary>
    /// Calculates the bobbing motion to simulate footsteps.
    /// </summary>
    /// <returns>
    /// Returns a Vector3 representing the bobbing position adjustment.
    /// </returns>
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
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

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
    /// Determines the target focus point for the camera to look at.
    /// Uses the camera pivot as a reference for position.
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
    /// Resets the camera position smoothly back to the starting position.
    /// This maintains a stable base when bobbing is disabled or motion stops.
    /// </summary>
    private void ResetPosition()
    {
        if (cameraBobber.localPosition == startPos) return;

        cameraBobber.localPosition = Vector3.Lerp(cameraBobber.localPosition, startPos, 1 * Time.deltaTime);
    }

    private void InspectObject(GameObject gameObject)
    {
        enabled = false;
    }
    
    private void FinishInspect()
    {
        enabled = true;
    }
}
