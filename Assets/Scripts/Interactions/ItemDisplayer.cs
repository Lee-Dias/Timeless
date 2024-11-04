using UnityEngine;
using UnityEngine.Events;

public class ItemDisplayer : MonoBehaviour
{
    public UnityEvent<Item> onItemDisplayed;
    public UnityEvent<Item> onTakeItem;
    [SerializeField] private Transform displayPos;
    private GameObject displayedObject;
    private Item displayedItem;

    public void DisplayItem(Item item)
    {
        if (displayedObject == null && displayedItem == null)
        {
            displayedObject = Instantiate(item.Prefab, displayPos.position, Quaternion.identity);
            displayedObject.transform.SetParent(displayPos);
            displayedItem = item;
            onItemDisplayed.Invoke(item);
        }
        else return;
    }

    public void TakeItem()
    {
        if (displayedObject != null && displayedItem != null)
        {
            onTakeItem.Invoke(displayedItem);
            displayedItem = null;
            Destroy(displayedObject);
            displayedObject = null;
        }
    }
}
