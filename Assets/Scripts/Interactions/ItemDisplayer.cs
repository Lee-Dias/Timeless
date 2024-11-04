using UnityEngine;
using UnityEngine.Events;

public class ItemDisplayer : MonoBehaviour
{
    public UnityEvent<Item> onItemDisplayed;
    [SerializeField] private Transform displayPos;
    private GameObject displayedItem;

    public void DisplayItem(Item item)
    {
        if (displayedItem == null)
        {
            displayedItem = Instantiate(item.Prefab, displayPos.position, Quaternion.identity);
            displayedItem.transform.SetParent(displayPos);
            onItemDisplayed.Invoke(item);
        }
        else return;
    }
}
