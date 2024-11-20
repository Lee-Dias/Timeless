using System;
using UnityEngine;

public class TabletButton : Interactable
{
    [SerializeField, Tooltip("The unique index for this button to identify it in the puzzle.")]
    private int buttonIndex;  // The unique index for this button to identify it in the puzzle.

    [SerializeField, Tooltip("Color of the button's glow when activated.")]
    private Color glowColor = Color.blue;  // Color of the button's glow when activated.

    [SerializeField, Tooltip("Glow intensity multiplier for the activated state.")]
    private float glowIntensity = 2.0f;  // How bright the glow is when activated (default is 2.0).

    public Action<int> activateButton;  // Action to trigger when the button is activated.
    public Action<int> deactivateButton;  // Action to trigger when the button is deactivated.

    public bool IsActive { get; private set; } = false;  // Flag to track the button's state.

    private Animator anim;  // Animator to handle button animations.
    private Light glowLight;  // Light component for glowing effect.
    private Material overlayMaterial;  // Material for button's emission (glowing effect).

    private void Awake()
    {
        // Ensure necessary components are present
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component missing from TabletButton.", this);
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.materials.Length > 1)
        {
            overlayMaterial = renderer.materials[1];  // Assuming the glow effect is the second material.
        }
        else
        {
            Debug.LogError("Overlay material for glow effect missing or incorrectly configured.", this);
        }

        // Attempt to get the glow light component, allowing for cases where it might not be present
        glowLight = GetComponentInChildren<Light>(true);
        if (glowLight != null)
        {
            glowLight.color = glowColor;
        }
        else
        {
            Debug.LogWarning("Glow light component missing in TabletButton, glow effect won't function.");
        }
    }

    private void OnEnable()
    {
        InteractEvent.AddListener(TFlipFlop);
    }

    private void OnDisable()
    {
        InteractEvent.RemoveListener(TFlipFlop);
    }

    /// <summary>
    /// Toggles the button's active state, activating or deactivating it and triggering respective effects.
    /// </summary>
    public void TFlipFlop()
    {
        if (IsActive)
        {
            DeactivateButton();
        }
        else
        {
            ActivateButton();
        }

        IsActive = !IsActive;  // Toggle the button's active state.
        anim.SetTrigger("Press");  // Play the button press animation.
    }

    /// <summary>
    /// Activates the button and triggers the associated visual effects and actions.
    /// </summary>
    private void ActivateButton()
    {
        activateButton?.Invoke(buttonIndex);
        SetGlowEffect(true);  // Enable glowing effect.
    }

    /// <summary>
    /// Deactivates the button and resets the visual effects.
    /// </summary>
    private void DeactivateButton()
    {
        deactivateButton?.Invoke(buttonIndex);
        SetGlowEffect(false);  // Disable glowing effect.
    }

    /// <summary>
    /// Sets the glow effect on the button by modifying the emission color and light.
    /// </summary>
    /// <param name="isActive">If true, activate the glow; otherwise, reset it.</param>
    private void SetGlowEffect(bool isActive)
    {
        if (overlayMaterial != null)
        {
            overlayMaterial.EnableKeyword("_EMISSION");
            overlayMaterial.SetColor("_EmissionColor", isActive ? glowColor * glowIntensity : Color.black);
        }

        if (glowLight != null)
        {
            glowLight.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// Disables interaction with the button when the puzzle is completed.
    /// </summary>
    public void OnPuzzleEnd()
    {
        CanInteract = false;  // Disable interaction with the button.
    }
}
