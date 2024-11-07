using UnityEngine;
using UnityEngine.Events;

public class ItemDisplayer : MonoBehaviour
{
    public UnityEvent<Item> onItemDisplayed;
    public UnityEvent<Item> onTakeItem;
    public UnityEvent<Item> onHasItem;
    private PlayerInventory playerInventory;
    [SerializeField] private Transform displayPos;
    private GameObject displayedObject;
    private Item displayedItem;

    private void Start(){
        //gets players inventory
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void HasItem(){
        //sees if there is an item on the displayer 
        if (displayedObject != null && displayedItem != null)
        {
            //takes item from displayer
            onTakeItem.Invoke(displayedItem);
            displayedItem = null;
            Destroy(displayedObject);
            displayedObject = null;
        }else{
            onHasItem.Invoke(displayedItem);
        }
    }

    public void DisplayItem()
    {
        Item item = playerInventory.GetSelectedItem();
        //sees if there is an item on the displayer 
        if (displayedObject == null && displayedItem == null)
        {
            //displays item
            displayedObject = Instantiate(item.Prefab, displayPos.position, Quaternion.identity);
            displayedObject.transform.SetParent(displayPos);
            displayedItem = item;
            onItemDisplayed.Invoke(item);
        }
        else return;
    }

}
