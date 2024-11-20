using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<Item> onItemAdded;
    
    public UnityEvent<Item> onItemRemoved;
    
    public UnityEvent<Item> onSelectedItemChanged;

    private List<Item> inventory;

    private int selectedItemIndex = 50;

    private void Awake()
    {
        inventory = new List<Item>();
    }

    /// <summary>
    /// Handles inventory navigation using the Q and E keys. 
    /// Updates the selected item index and triggers the onSelectedItemChanged event if the selected item changes.
    /// </summary>
    private void Update()
    {
        // Skip selection logic if the inventory is empty
        if (inventory.Count == 0) return;

        int lastIndex = selectedItemIndex;

        // Move selection left with Q key and wrap around
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedItemIndex = (selectedItemIndex - 1 + inventory.Count) % inventory.Count;
            Debug.Log($"Selected Item Index: {selectedItemIndex}");
        }
        // Move selection right with E key and wrap around
        else if (Input.GetKeyDown(KeyCode.E))
        {
            selectedItemIndex = (selectedItemIndex + 1) % inventory.Count;
            Debug.Log($"Selected Item Index: {selectedItemIndex}");
        }

        // Trigger event if the selected item has changed
        if (selectedItemIndex != lastIndex)
        {
            onSelectedItemChanged?.Invoke(inventory[selectedItemIndex]);
        }
    }

    /// <summary>
    /// Adds an item to the inventory if it is not already present, 
    /// and triggers the onItemAdded event.
    /// </summary>
    /// <param name="item">The item to add to the inventory.</param>
    public void AddItemToInventory(Item item)
    {
        if (!HasItem(item))
        {
            inventory.Add(item);
            Debug.Log($"{item.ID} | {item.Name} has been added to the inventory");

            onItemAdded?.Invoke(item);
        }
    }

    /// <summary>
    /// Retrieves the currently selected item from the inventory.
    /// </summary>
    /// <returns>The currently selected item, or null if none is selected.</returns>
    public Item GetSelectedItem()
    {
        if (inventory.Count > 0 && selectedItemIndex < inventory.Count)
        {
            return inventory[selectedItemIndex];
        }
        return null;
    }

    /// <summary>
    /// Removes an item from the inventory if it is present,
    /// and triggers the onItemRemoved event. Adjusts the selected item index if needed.
    /// </summary>
    /// <param name="item">The item to remove from the inventory.</param>
    public void RemoveItemFromInventory(Item item)
    {
        if (HasItem(item))
        {
            inventory.Remove(item);
            Debug.Log($"{item.ID} | {item.Name} has been removed from the inventory");
            onItemRemoved?.Invoke(item);
            // Adjust the selected item index if it exceeds the inventory count
            if (selectedItemIndex >= inventory.Count)
            {
                selectedItemIndex = Mathf.Max(0, inventory.Count - 1);
            }
        }
    }

    /// <summary>
    /// Checks if a specific item exists in the inventory.
    /// </summary>
    /// <param name="item">The item to check for in the inventory.</param>
    /// <returns>True if the item is in the inventory, otherwise false.</returns>
    public bool HasItem(Item item)
    {
        return inventory.Contains(item);
    }
}
