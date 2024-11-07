using UnityEngine;
using UnityEngine.Events;

public class SelectedItemGrabber : MonoBehaviour
{
    private PlayerInventory playerInventory;
    public UnityEvent<Item> selectedItem;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void GrabSelectedItem()
    {
        Item selected = playerInventory.GetSelectedItem();
        if (selected != null)   
            selectedItem.Invoke(selected);
    }
}