using System.Collections.Generic;
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
public class PlayerUIInventory : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject inventorySlotPrefab; // Prefab used for creating inventory slots

    [Header("Debugging")]

    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;

    private Animator anim; // Animator for triggering UI animations
    private Dictionary<int, InventorySlotUI> iconDictionary; // Dictionary to track inventory slots by item ID

    private void Awake()
    {
        // Initialize the icon dictionary
        iconDictionary = new Dictionary<int, InventorySlotUI>();

        // Cache the Animator component
        anim = GetComponent<Animator>();

        // Ensure the inventorySlotPrefab is assigned
        if (inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory slot prefab is not assigned! Please assign it in the inspector.");
            return;
        }

        Log("PlayerUIInventory initialized successfully.");
    }

    /// <summary>
    /// Updates the UI to reflect the currently selected item.
    /// Highlights the selected item and unselects others.
    /// </summary>
    /// <param name="item">The item to highlight as selected.</param>
    public void ChangeSelectedItem(Item item)
    {
        foreach (InventorySlotUI slot in iconDictionary.Values)
        {
            bool isSelected = slot.ID == item.ID;
            slot.GetComponent<Animator>().SetBool("Selected", isSelected);
        }
        Log($"Selected item updated: {item.Name} (ID: {item.ID})");
    }

    /// <summary>
    /// Adds a new item slot to the UI.
    /// If the item is already present, the method does nothing.
    /// </summary>
    /// <param name="item">The item to add to the UI.</param>
    public void AddSlotToUI(Item item)
    {
        // Check if the item already exists in the inventory
        if (iconDictionary.ContainsKey(item.ID))
        {
            Log($"Item {item.Name} (ID: {item.ID}) already exists in the UI.");
            return;
        }

        // Instantiate new slot and retrieve its UI component
        InventorySlotUI newSlot = Instantiate(inventorySlotPrefab).GetComponent<InventorySlotUI>();
        if (newSlot == null)
        {
            Debug.LogError("InventorySlotUI component missing on the inventory slot prefab!");
            Destroy(newSlot); // Clean up to prevent orphaned objects
            return;
        }

        // Set item data and initialize the UI slot
        newSlot.SetItemUI(item);

        // Set the parent to maintain UI hierarchy
        newSlot.transform.SetParent(transform);

        // Trigger "In" animation for adding the slot
        anim.SetTrigger("In");

        // Add the new slot to the dictionary
        iconDictionary[item.ID] = newSlot;
        Log($"Added new slot to UI for item: {item.Name} (ID: {item.ID})");
    }

    /// <summary>
    /// Removes an item slot from the UI.
    /// If the item is not found, the method does nothing.
    /// </summary>
    /// <param name="item">The item to remove from the UI.</param>
    public void RemoveUISlot(Item item)
    {
        // Try to find the slot in the dictionary
        if (iconDictionary.TryGetValue(item.ID, out InventorySlotUI slot))
        {
            // Destroy the slot's GameObject and remove it from the dictionary
            Destroy(slot.gameObject);
            iconDictionary.Remove(item.ID);
            Log($"Removed UI slot for item: {item.Name} (ID: {item.ID})");
        }
        else
        {
            Debug.LogWarning($"Attempted to remove non-existent item from UI: {item.Name} (ID: {item.ID})");
        }
    }

    public void SetActive(bool b)
    {
        foreach (InventorySlotUI inventorySlotUI in iconDictionary.Values)
        {
            inventorySlotUI.SetActive(b);
        }
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
            if (identifyObject) Debug.Log(message, this); // Includes object's name in the Console
            else Debug.Log(message); // Logs message without object identifier
        }
    }
}
