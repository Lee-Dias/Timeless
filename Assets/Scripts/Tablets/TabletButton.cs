using System;
using System.Collections.Generic;
using UnityEngine;

public class TabletButton : Interactable
{
    [SerializeField] private Renderer BackBrick;

    [SerializeField, Tooltip("The unique index for this button to identify it in the puzzle.")]
    private int buttonIndex;  // The unique index for this button to identify it in the puzzle.

    public Action<int> activateButton;  // Action to trigger when the button is activated.
    public Action<int> deactivateButton;  // Action to trigger when the button is deactivated.

    public bool IsActive { get; private set; } = false;  // Flag to track the button's state.

    private AudioSource audioSource;  // Audio source to play button press sound.
    private Animator anim;  // Animator to handle button animations.
    [SerializeField] private Material glowMaterial;  // Material for button's emission (glowing effect).
    private Material originalMaterial;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component missing from TabletButton.", this);
        }
        // Ensure necessary components are present
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator component missing from TabletButton.", this);
        }

        if (BackBrick == null)
        {
            Debug.LogWarning("BackBrick Renderer missing or incorrectly configured.", this);
        }
        else
        {
            originalMaterial = BackBrick.material;
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
        anim?.SetTrigger("Press");  // Play the button press animation.
        if (audioSource != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1f);  // Randomize the pitch.
            audioSource.Play();  // Play the button press sound.
        }
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
    /// </summary>s
    /// <param name="isActive">If true, activate the glow; otherwise, reset it.</param>
    private void SetGlowEffect(bool isActive)
    {
        if (isActive)
        {
            BackBrick.SetMaterials(new List<Material>() { glowMaterial });
        }
        else
        {
            BackBrick.SetMaterials(new List<Material>() { originalMaterial });
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
