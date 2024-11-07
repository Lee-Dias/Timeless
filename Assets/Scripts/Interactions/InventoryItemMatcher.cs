using UnityEngine;
using UnityEngine.Events;

public class InventoryItemMatcher : MonoBehaviour
{
    private PlayerInventory playerInventory;
    public UnityEvent<Item> selectedItem;
    [SerializeField]private Item.Era era;
    [SerializeField]private Item rightItem;
    private bool isRightItem = false;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }
    public void CheckItem()
    {
        Item selected = playerInventory.GetSelectedItem();
        if (selected != null){     
            if (era == selected.era){
                if(rightItem == selected){
                    Debug.Log("right item");
                    isRightItem = true ;
                }
                selectedItem.Invoke(selected);
            }
            else
            {
                Debug.Log("wrong era");
            }
        }
    }
    public bool HasRightItem(){
        return isRightItem;
    }
}