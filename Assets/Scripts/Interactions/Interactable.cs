using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an interactable object in the game world, providing functionality for triggering events, 
/// managing inventory additions, and handling single-use interactions.
/// </summary>
/// <remarks>
/// The `Interactable` class enables objects to respond to player interactions. When interacted with, 
/// the object can trigger custom events, add items to the player's inventory, or deactivate itself 
/// based on its configuration. This class is highly versatile and can be used for various gameplay 
/// mechanics, such as pickups, switches, or interactive environment elements.
/// 
/// <para><b>Key Features:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Triggers custom Unity events upon interaction.</description>
/// </item>
/// <item>
/// <description>Optionally adds items to the player's inventory.</description>
/// </item>
/// <item>
/// <description>Supports single-use interactions by disabling itself after the first use.</description>
/// </item>
/// <item>
/// <description>Configurable interaction logic using Unity's inspector.</description>
/// </item>
/// </list>
/// 
/// <para><b>How It Works:</b></para>
/// <para>
/// When the <c>Interact</c> method is called (typically by a player interaction system), 
/// the script checks if the object is marked as interactable using the <c>CanInteract</c> property. 
/// If interactable, the following actions may occur based on the configuration:
/// </para>
/// <list type="number">
/// <item>
/// <description>Any events assigned to <c>InteractEvent</c> are invoked.</description>
/// </item>
/// <item>
/// <description>If the <c>addToInv</c> property is true, the associated item is added to the player's inventory, 
/// and the object is deactivated.</description>
/// </item>
/// <item>
/// <description>If the <c>interactOnce</c> property is true, the object becomes non-interactable after the first use.</description>
/// </item>
/// </list>
/// 
/// <para><b>Dependencies:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Requires a <c>PlayerInventory</c> script to manage inventory additions.</description>
/// </item>
/// <item>
/// <description>Optionally interacts with an <c>Item</c> class for inventory logic.</description>
/// </item>
/// </list>
/// 
/// <para><b>Inspector Configurations:</b></para>
/// <list type="bullet">
/// <item>
/// <description><c>addToInv</c>: Determines whether the object should be added to the inventory upon interaction.</description>
/// </item>
/// <item>
/// <description><c>item</c>: The associated item to add to the inventory, displayed only when <c>addToInv</c> is true.</description>
/// </item>
/// <item>
/// <description><c>interactOne</c>: Enables single-use interaction behavior.</description>
/// </item>
/// <item>
/// <description><c>CanInteract</c>: Indicates whether the object is currently available for interaction.</description>
/// </item>
/// </list>
/// 
/// <para><b>Unity Events:</b></para>
/// <list type="bullet">
/// <item>
/// <description><c>InteractEvent</c>: Invoked whenever the object is interacted with.</description>
/// </item>
/// </list>
/// 
/// </remarks>
public class Interactable : MonoBehaviour
{

    [SerializeField, Tooltip("Determines whether the object should be added to the player's inventory upon interaction.")]
    private bool addToInv = false;

    [SerializeField, ShowIf(nameof(addToInv)), Tooltip("The item to be added to the inventory if true.")]
    private Item item;

    [SerializeField, Tooltip("Determines whether the object can only be interacted with once.")]
    private bool interactOnce = false;

    [SerializeField] private AudioClip[] pickUpSounds;
    [SerializeField] private float volume = 1f;
    [SerializeField] private AudioSource audioPrefab;

    [HideInInspector] public bool CanInteract = true;

    [Tooltip("Event triggered when the object is interacted with.")]
    public UnityEvent InteractEvent;

    private InspectionsHandler inspectionsHandler;

    /// <summary>
    /// Handles the interaction with the object.
    /// Triggers the <see cref="InteractEvent"/> and manages inventory addition or single-use behavior.
    /// </summary>
    public void Interact()
    {
        inspectionsHandler = FindFirstObjectByType<InspectionsHandler>();

        // Only proceed if the object is marked as interactable and the player isnt inspecting.
        if (CanInteract && !inspectionsHandler.inspecting)
        {
            // Invoke any assigned interaction events.
            InteractEvent?.Invoke();

            // Add the object to the player's inventory if applicable.
            if (addToInv)
            {
                FindFirstObjectByType<PlayerInventory>()?.AddItemToInventory(item);
                // Disable the game object after adding to the inventory.
                gameObject.SetActive(false);
            }
            // Disable future interactions if it is a single-use interaction.
            if (interactOnce) CanInteract = false;

            if (pickUpSounds != null && pickUpSounds.Length > 0 && audioPrefab != null) 
            {
                AudioClip clip = pickUpSounds[Random.Range(0, pickUpSounds.Length)];
                Instantiate(audioPrefab, transform.position, Quaternion.identity).PlayOneShot(clip, volume);
            }
        }
    }
}

