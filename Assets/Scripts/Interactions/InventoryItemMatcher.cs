using UnityEngine;
using UnityEngine.Events;

public class InventoryItemMatcher : MonoBehaviour
{
    private PlayerInventory playerInventory;
    public UnityEvent onHasItem;
    public UnityEvent onDontHaveItem;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void CheckItem(Item item)
    {
        if (playerInventory.HasItem(item))
        {
            onHasItem.Invoke();
        }
        else onDontHaveItem.Invoke();
    }
}