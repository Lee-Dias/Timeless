using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<Item> onItemAdded;
    public UnityEvent<Item> onItemRemoved;
    private Dictionary<int, Item> inventory;

    private void Awake()
    {
        inventory = new Dictionary<int, Item>();
    }
    public void AddItemToInventory(Item item)
    {
        if (!HasItem(item))
        {
            inventory.Add(item.ID, item);
            Debug.Log($"{item.ID} | {item.Name} has been added to the inventory");

            onItemAdded.Invoke(item);
        }
    }

    public void RemoveItemFromInventory(Item item)
    {
        if (HasItem(item))
        {
            inventory.Remove(item.ID);
            Debug.Log($"{item.ID} | {item.Name} has been removed from the inventory");

            onItemRemoved.Invoke(item);
        }
    }

    public bool HasItem(Item item)
    {
        if (inventory.ContainsValue(item))
        {
            return true;
        }
        else return false;
    }

    public bool HasItem(int id)
    {
        if (inventory.ContainsKey(id))
        {
            return true;
        }
        else return false;
    }
}
