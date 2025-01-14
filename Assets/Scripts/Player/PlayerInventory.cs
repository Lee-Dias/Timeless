using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

/// <summary>
/// Manages the player's inventory, including adding, removing, and selecting items. 
/// It also raises UnityEvents for item added, removed, and selection changes.
///
/// The player can interact with the inventory through the following actions:
/// <list type="bullet">
///     <item><description>Q key: Selects the previous item in the inventory.</description></item>
///     <item><description>E key: Selects the next item in the inventory.</description></item>
/// </list>
/// 
/// <para>
/// The following UnityEvents are raised:
/// <list type="bullet">
///     <item><description>onItemAdded: Raised when an item is added to the inventory.</description></item>
///     <item><description>onItemRemoved: Raised when an item is removed from the inventory.</description></item>
///     <item><description>onSelectedItemChanged: Raised when the selected item changes.</description></item>
/// </list>
/// </para>
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("Events")]

    // Event raised when an item is added to the inventory.
    public UnityEvent<Item> onItemAdded;

    // Event raised when an item is removed from the inventory.
    public UnityEvent<Item> onItemRemoved;

    // Event raised when the selected item in the inventory changes.
    public UnityEvent<Item> onSelectedItemChanged;

    private List<Item> inventory; // List that stores the player's items.

    private int selectedItemIndex = -1; // The index of the currently selected item, initialized to -1 (no selection).

    [Header("Debugging")]

    /// <summary>
    /// Flag to enable or disable debug messages from this script.
    /// </summary>
    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    /// <summary>
    /// Flag to include the object's name in debug messages, if debugging is enabled.
    /// </summary>
    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;


    private bool canChangeSelection = true;

    private void Awake()
    {
        // Initialize the inventory as an empty list.
        inventory = new List<Item>();
    }

    private void Update()
    {
        if (inventory.Count == 0 || !canChangeSelection) return; // Skip if there are no items in the inventory.

        int lastIndex = selectedItemIndex;

        // Handle item selection changes using the Q and E keys.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedItemIndex = (selectedItemIndex - 1 + inventory.Count) % inventory.Count;
            Log($"Selected Item Index: {selectedItemIndex}");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            selectedItemIndex = (selectedItemIndex + 1) % inventory.Count;
            Log($"Selected Item Index: {selectedItemIndex}");
        }

        // If the selected item has changed, invoke the event for selection change.
        if (selectedItemIndex != lastIndex)
        {
            onSelectedItemChanged?.Invoke(inventory[selectedItemIndex]);
        }
    }

    /// <summary>
    /// Adds an item to the inventory if it doesn't already exist.
    /// Automatically selects the newly added item.
    /// </summary>
    /// <param name="item">The item to add to the inventory.</param>
    public void AddItemToInventory(Item item)
    {
        // Only add the item if it doesn't already exist in the inventory.
        if (!HasItem(item))
        {
            inventory.Add(item);
            Log($"{item.ID} | {item.Name} has been added to the inventory");

            // Raise the event for item added.
            onItemAdded?.Invoke(item);

            // Automatically select the newly added item.
            selectedItemIndex = inventory.Count - 1;
            onSelectedItemChanged?.Invoke(inventory[selectedItemIndex]);
        }
    }

    public void AddItemsToInventory(IEnumerable<Item> items)
    {
        foreach(Item item in items)
        {
            AddItemToInventory(item);
        }
    }

    public void RemoveItemsFromInventory(IEnumerable<Item> items)
    {
        foreach(Item item in items)
        {
            RemoveItemFromInventory(item);
        }
    }

    public void RemoveAllItemsFromInventory()
    {
        foreach(Item item in inventory)
        {
            RemoveItemFromInventory(item);
        }
    }

    /// <summary>
    /// Removes an item from the inventory and adjusts the selected item accordingly.
    /// </summary>
    /// <param name="item">The item to remove from the inventory.</param>
    public void RemoveItemFromInventory(Item item)
    {
        // Only remove the item if it exists in the inventory.
        if (HasItem(item))
        {
            int removedIndex = inventory.IndexOf(item);
            inventory.Remove(item);

            Log($"{item.ID} | {item.Name} has been removed from the inventory");

            // Raise the event for item removed.
            onItemRemoved?.Invoke(item);

            // Handle selection adjustment after item removal.
            if (inventory.Count == 0)
            {
                selectedItemIndex = -1; // Reset selection if inventory is empty.
            }
            else if (selectedItemIndex == removedIndex)
            {
                // Select the previous item if possible, otherwise the first item.
                selectedItemIndex = Mathf.Clamp(removedIndex - 1, 0, inventory.Count - 1);
                onSelectedItemChanged?.Invoke(inventory[selectedItemIndex]);
            }
            else if (selectedItemIndex > removedIndex)
            {
                // Shift selection index left if an earlier item was removed.
                selectedItemIndex--;
            }
        }
    }

    /// <summary>
    /// Checks whether the inventory contains a specific item.
    /// </summary>
    /// <param name="item">The item to check for in the inventory.</param>
    /// <returns>True if the item is in the inventory, false otherwise.</returns>
    public bool HasItem(Item item)
    {
        return inventory.Contains(item);
    }

    /// <summary>
    /// Gets the currently selected item from the inventory.
    /// </summary>
    /// <returns>The currently selected item, or null if no item is selected.</returns>
    public Item GetSelectedItem()
    {
        // Return the selected item if there is one.
        if (inventory.Count > 0 && selectedItemIndex >= 0 && selectedItemIndex < inventory.Count)
        {
            return inventory[selectedItemIndex];
        }
        return null; // Return null if no item is selected.
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
            if (identifyObject) Debug.Log(message, this); // Logs with object name if 'identifyObject' is true.
            else Debug.Log(message); // Logs message without object name.
        }
    }

    public void SetCanChangeSelection(bool value)
    {
        canChangeSelection = value;
    }
}
