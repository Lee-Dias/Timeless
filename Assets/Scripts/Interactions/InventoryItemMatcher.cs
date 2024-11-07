using UnityEngine;
using UnityEngine.Events;

public class InventoryItemMatcher : MonoBehaviour
{
    private PlayerInventory playerInventory;
    public UnityEvent<Item> selectedItem;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void CheckItem()
    {
        Item selected = playerInventory.GetSelectedItem();
        if (selected != null)   
            selectedItem.Invoke(selected);
    }
}