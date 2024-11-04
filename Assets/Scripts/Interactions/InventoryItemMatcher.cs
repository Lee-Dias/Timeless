using UnityEngine;
using UnityEngine.Events;

public class InventoryItemMatcher : MonoBehaviour
{
    private PlayerInventory playerInventory;
    public UnityEvent<Item> onHasItem;
    public UnityEvent<Item> onDontHaveItem;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void CheckItem(Item item)
    {
        if (playerInventory.HasItem(item))
        {
            onHasItem.Invoke(item);
        }
        else onDontHaveItem.Invoke(item);
    }
}