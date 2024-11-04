using System.Collections.Generic;
using UnityEngine;

public class PlayerUIInventory : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject inventorySlotPrefab;
    private Dictionary<int, GameObject> iconDictionary;

    private void Awake()
    {
        iconDictionary = new Dictionary<int, GameObject>();

        anim = GetComponent<Animator>();

        if (inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory slot prefab is not assigned!");
            return;
        }
    }

    public void AddSlotToUI(Item item)
    {
        // Check if the item already exists in the inventory by ID
        if (iconDictionary.ContainsKey(item.ID)) return;

        // Instantiate new slot, assign properties, and add to dictionary
        GameObject newSlot = Instantiate(inventorySlotPrefab);
        InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

        if (slotUI == null)
        {
            Debug.LogError("InventorySlotUI component missing on inventorySlotPrefab!");
            Destroy(newSlot); // Clean up in case of error
            return;
        }

        slotUI.ItemName = item.Name;
        slotUI.Icon.sprite = item.Icon;
        slotUI.ID = item.ID;

        // Set parent and trigger animation
        newSlot.transform.SetParent(transform);
        anim.SetTrigger("In");

        // Add the new slot to the dictionary
        iconDictionary[item.ID] = newSlot;
    }

    public void RemoveUISlot(Item item)
    {
        // Check if the item exists in the dictionary
        if (iconDictionary.TryGetValue(item.ID, out GameObject icon))
        {
            Destroy(icon);
            iconDictionary.Remove(item.ID); // Remove from dictionary after destroying
        }
    }
}
