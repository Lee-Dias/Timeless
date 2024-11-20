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
        //gets players inventory
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }


    public void CheckItem()
    {
        //gets item that the player has in hand
        Item selected = playerInventory.GetSelectedItem();
        //confirms if he has an item
        if (selected != null){     
            //sees if item has the right era
            if (era == selected.era){
                //sees if it is the right era
                if(rightItem == selected){
                    Debug.Log("right item");
                    isRightItem = true ;
                }
                //display item on screen
                selectedItem.Invoke(selected);
            }
            else
            {
                Debug.Log("wrong era");
            }
        }
    }
    
    //sees if the right item is on the displayer
    public bool HasRightItem(){
        return isRightItem;
    }

    //makes isrightitem false if no item is there
    public void NoItemOnDisplayer(){
        isRightItem = false;
    }
}