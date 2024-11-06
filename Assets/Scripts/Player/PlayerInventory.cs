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

    private int selectedItemIndex = 0;

    private void Awake()
    {
        inventory = new List<Item>();
    }

    private void Update()
    {
        if (inventory.Count == 0) return;  // Skip if inventory is empty

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
        }
    }

    public Item GetSelectedItem()
    {
        if (inventory.Count > 0 && selectedItemIndex < inventory.Count)
        {
            return inventory[selectedItemIndex];
        }
        return null;
    }

    public void RemoveItemFromInventory(Item item)
    {
        if (HasItem(item))
        {
            inventory.Remove(item);
            Debug.Log($"{item.ID} | {item.Name} has been removed from the inventory");

            onItemRemoved?.Invoke(item);

            if (selectedItemIndex >= inventory.Count)
            {
                selectedItemIndex = Mathf.Max(0, inventory.Count - 1);
            }
        }
    }

    public bool HasItem(Item item)
    {
        return inventory.Contains(item);
    }
}
