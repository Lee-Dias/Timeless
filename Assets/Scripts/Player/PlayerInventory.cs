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

    private int selectedItemIndex = -1; // Initialize with -1 to indicate no selection initially.

    private void Awake()
    {
        inventory = new List<Item>();
    }

    private void Update()
    {
        if (inventory.Count == 0) return;

        int lastIndex = selectedItemIndex;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedItemIndex = (selectedItemIndex - 1 + inventory.Count) % inventory.Count;
            Debug.Log($"Selected Item Index: {selectedItemIndex}");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            selectedItemIndex = (selectedItemIndex + 1) % inventory.Count;
            Debug.Log($"Selected Item Index: {selectedItemIndex}");
        }

        if (selectedItemIndex != lastIndex)
        {
            onSelectedItemChanged?.Invoke(inventory[selectedItemIndex]);
        }
    }

    public void AddItemToInventory(Item item)
    {
        if (!HasItem(item))
        {
            inventory.Add(item);
            Debug.Log($"{item.ID} | {item.Name} has been added to the inventory");

            onItemAdded?.Invoke(item);

            // Automatically select the newly added item
            selectedItemIndex = inventory.Count - 1;
            onSelectedItemChanged?.Invoke(inventory[selectedItemIndex]);
        }
    }

    public Item GetSelectedItem()
    {
        if (inventory.Count > 0 && selectedItemIndex >= 0 && selectedItemIndex < inventory.Count)
        {
            return inventory[selectedItemIndex];
        }
        return null;
    }

    public void RemoveItemFromInventory(Item item)
    {
        if (HasItem(item))
        {
            int removedIndex = inventory.IndexOf(item);
            inventory.Remove(item);

            Debug.Log($"{item.ID} | {item.Name} has been removed from the inventory");
            onItemRemoved?.Invoke(item);

            if (inventory.Count == 0)
            {
                selectedItemIndex = -1; // Reset selection if inventory is empty
            }
            else if (selectedItemIndex == removedIndex)
            {
                // Select the previous item if possible, otherwise the first item
                selectedItemIndex = Mathf.Clamp(removedIndex - 1, 0, inventory.Count - 1);
                onSelectedItemChanged?.Invoke(inventory[selectedItemIndex]);
            }
            else if (selectedItemIndex > removedIndex)
            {
                // Shift selection index left if an earlier item was removed
                selectedItemIndex--;
            }
        }
    }

    public bool HasItem(Item item)
    {
        return inventory.Contains(item);
    }
}
